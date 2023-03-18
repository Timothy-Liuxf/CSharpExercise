# Grammar of GoScript

```ANTLR
Program                 : Statement*
                        ;

Statement               : VarDecl ';'? NEWLINE
                        | CompoundStmt
                        | Expression? ';'? NEWLINE
                        ;

CompoundStmt            : '{' NEWLINE Statement* '}' NEWLINE
                        ;

VarDecl                 : 'var' IdentifierList Type ('=' ExpressionList)?
                        | 'var' IdentifierList '=' ExpressionList
                        ;

IdentifierList          : Identifier (',' Identifier)*
                        ;

ExpressionList          : Expression (',' Expression)*
                        ;

Type                    : TYPE_KEYWORD
                        ;

Expression              : AdditiveExpr
                        ;

AdditiveExpr            : AdditiveExpr ('+'|'-') MultiplicativeExpr
                        | MultiplicativeExpr
                        ;

MultiplicativeExpr      : MultiplicativeExpr ('*'|'/'|'%') UnaryExpr
                        | UnaryExpr
                        ;

UnaryExpr               : '-' UnaryExpr
                        : PrimaryExpr
                        ;

PrimaryExpr             : IDENTIFIER
                        | INTEGER_LITERAL
                        | '(' Expression ')'
                        ;
```
