# Grammar

```ANTLR
Program         : Statement*
                ;

Statement       : VarDecl ';'? NEWLINE
                | Expression? ';'? NEWLINE
                ;

VarDecl         : 'var' IDENTIFIER Type ('=' Expression)?
                | 'var' IDENTIFIER '=' Expression
                ;

Type            : TYPE_KEYWORD
                ;

Expression      : AdditiveExpr
                ;

AdditiveExpr    : AdditiveExpr ('+'|'-') Multiplicative
                | Multiplicative
                ;

Multiplicative  : Multiplicative ('*'|'/'|'%') PrimaryExpr
                | PrimaryExpr
                ;

PrimaryExpr     : IDENTIFIER
                | INTEGER_LITERAL
                | '(' Expression ')'
                ;
```
