# Grammar

```ANTLR
Program         : Statement*
                ;

Statement       : VarDecl ';'? NEWLINE
                | Expression? ';'? NEWLINE
                ;

Expression      : AddExpr
                ;

VarDecl         : 'var' IDENTIFIER Type ('=' Expression)?
                | 'var' IDENTIFIER '=' Expression
                ;

Type            : TYPE_KEYWORD
                ;

AddExpr         : AddExpr '+' PrimaryExpr
                | PrimaryExpr
                ;

PrimaryExpr     : IDENTIFIER
                | INTEGER_LITERAL
                ;
```
