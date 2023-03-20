# Grammar of GoScript

```ANTLR
Program                 : Statement*
                        ;

Statement               : VarDecl ';'? NEWLINE
                        | CompoundStmt
                        | AssignOrExprStmt
                        ;

AssignOrExprStmt        : IdExprList '=' ExpressionList ';'? NEWLINE
                        | IdExprList ':=' ExpressionList ';'? NEWLINE
                        | Expression? ';'? NEWLINE
                        ;

IdExprList              : IdExpr (',' IdExpr)*
                        ;

IdExpr                  : IDENTIFIER
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

LogicalOrExpr           : LogicalOrExpr '||' LogicalAndExpr
                        | LogicalAndExpr
                        ;

LogicalAndExpr          : LogicalAndExpr '&&' AdditiveExpr
                        | AdditiveExpr
                        ;

AdditiveExpr            : AdditiveExpr ('+'|'-') MultiplicativeExpr
                        | MultiplicativeExpr
                        ;

MultiplicativeExpr      : MultiplicativeExpr ('*'|'/'|'%') UnaryExpr
                        | UnaryExpr
                        ;

UnaryExpr               : '-' UnaryExpr
                        : '!' UnaryExpr
                        : PrimaryExpr
                        ;

PrimaryExpr             : IdExpr
                        | INTEGER_LITERAL
                        | BOOL_LITERAL
                        | '(' Expression ')'
                        ;
```
