# EF Core 3.x support for SQL Server's `TRY_PARSE` function
## Use

Install the NuGet package:
```powershell
Install-Package NevaleeBusinessSolutions.EntityFrameworkCore.SqlServer.TryParse
```

Register the functions in your `DbContext`'s `OnModelCreating` method:
```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);
    TryParse.Register(modelBuilder);
}
```

Then call the functions as part of a query:
```csharp
var result = context.Set<SomeEntity>()
    .Select(e => new { e.Id, e.Value, ValueInt32 = TryParse.Int32(e.Value) })
    .ToList();
```

This will generate the expected SQL:
```sql
SELECT Id, Value, TRY_PARSE(Value As int) As ValueInt32 FROM SomeEntities
```

## Background
[TRY_PARSE](https://docs.microsoft.com/en-us/sql/t-sql/functions/try-parse-transact-sql) was added in SQL Server 2012. However, EF Core 3.x does not support calling this function by default.

Whilst EF Core provides methods to [map user-defined functions](https://docs.microsoft.com/en-us/ef/core/querying/database-functions), mapping `TRY_PARSE` is complicated by the way the arguments are passed. EF Core has great support for traditional functions, where the arguments are passed as a comma-separated list - eg:
```sql
dbo.SomeFunction(Foo.Bar, @b, 42)
```

But for `TRY_PARSE`, the arguments are separated by spaces, not commas:
```sql
TRY_PARSE(Foo.Bar AS int)
```

To enable this, it was necessary to implement a custom `SqlExpression` class to represent the parameter. This class needs to override both the `Print` and `Accept` methods in order to generate the correct SQL.

```csharp
internal sealed class TryParseArgumentExpression : SqlExpression
{
    private readonly SqlExpression _sourceExpression;
    private readonly SqlFragmentExpression _asExpression;

    public TryParseArgumentExpression(Type type, SqlExpression sourceExpression, string sqlTypeName) 
        : base(type, sourceExpression.TypeMapping)
    {
        _sourceExpression = sourceExpression ?? throw new ArgumentNullException(nameof(sourceExpression));
        _asExpression = new SqlFragmentExpression($" AS {sqlTypeName}");
    }

    private TryParseArgumentExpression(Type type, SqlExpression sourceExpression, SqlFragmentExpression asExpression) 
        : base(type, sourceExpression.TypeMapping)
    {
        _sourceExpression = sourceExpression ?? throw new ArgumentNullException(nameof(sourceExpression));
        _asExpression = asExpression ?? throw new ArgumentNullException(nameof(asExpression));
    }

    protected override Expression VisitChildren(ExpressionVisitor visitor)
    {
        var newSource = (SqlExpression?)visitor.Visit(_sourceExpression) ?? _sourceExpression;
        var newAsExpression = (SqlFragmentExpression?)visitor.Visit(_asExpression) ?? _asExpression;
        if (Equals(newSource, _sourceExpression) && Equals(newAsExpression, _asExpression)) return this;
        return new TryParseArgumentExpression(Type, newSource, newAsExpression);
    }

    protected override Expression Accept(ExpressionVisitor visitor)
    {
        visitor.Visit(_sourceExpression);
        visitor.Visit(_asExpression);
        return this;
    }

    public override void Print(ExpressionPrinter expressionPrinter)
    {
        expressionPrinter.Visit(_sourceExpression);
        expressionPrinter.Visit(_asExpression);
    }
}
```

It was then possible to use this custom expression, along with an internal attribute which specifies the mapped SQL type name, to register the custom functions:
```csharp
public static void Register(ModelBuilder modelBuilder)
{
    foreach (var dbFunc in typeof(TryParse).GetMethods(BindingFlags.Public | BindingFlags.Static))
    {
        var attribute = dbFunc.GetCustomAttribute<SqlTypeNameAttribute>();
        if (attribute is null) continue;

        modelBuilder.HasDbFunction(dbFunc).HasTranslation(args =>
        {
            var newArgs = args.ToList();
            newArgs[0] = new TryParseArgumentExpression(dbFunc.ReturnType, newArgs[0], attribute.SqlTypeName);
            return SqlFunctionExpression.Create("TRY_PARSE", newArgs, dbFunc.ReturnType, null);
        });
    }
}
```

## License

Copyright (c) 2021 Richard Deeming All rights reserved.

This code is free software: you can redistribute it and/or modify it under the terms of either

    the Code Project Open License (CPOL) version 1 or later; or
    the GNU General Public License as published by the Free Software Foundation, version 3 or later; or
    the BSD 2-Clause License;

This code is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
