# Grammar of GoScript

```ANTLR
Program                 : Statement*
                        ;

Statement               : VarDeclStmt
                        | CompoundStmt
                        | IfStmt
                        | ForStmt
                        | BreakStmt
                        | ContinueStmt
                        | AssignOrExprStmt
                        ;

AssignOrExprStmt        : AssignOrExpr ';'? NEWLINE
                        ;

AssignOrExpr            : IdExprList '=' ExpressionList
                        | IdExprList ':=' ExpressionList
                        | Expression?
                        ;

IdExprList              : IdExpr (',' IdExpr)*
                        ;

IdExpr                  : IDENTIFIER
                        ;

CompoundStmt            : Compound NEWLINE
                        ;

Compound                : '{' NEWLINE Statement* '}'
                        ;

VarDeclStmt             : VarDecl ';'? NEWLINE
                        ;

VarDecl                 : 'var' IdentifierList Type ('=' ExpressionList)?
                        | 'var' IdentifierList '=' ExpressionList
                        ;

IfStmt                  : 'if' Expression CompoundStmt ('else' 'if' Expression CompoundStmt)* ('else' CompoundStmt)?
                        ;

ForStmt                 : 'for' AssignOrExpr ';' Expr ';' Expr CompoundStmt
                        | 'for' Expr CompoundStmt
                        | 'for' CompoundStmt
                        ;

BreakStmt               : 'break' ';'? NEWLINE
                        ;

ContinueStmt            : 'continue' ';'? NEWLINE
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

LogicalAndExpr          : LogicalAndExpr '&&' ComparisonExpr
                        | ComparisonExpr
                        ;

ComparisonExpr          : ComparisonExpr '==' AdditiveExpr
                        | ComparisonExpr '!=' AdditiveExpr
                        | ComparisonExpr '<' AdditiveExpr
                        | ComparisonExpr '>' AdditiveExpr
                        | ComparisonExpr '<=' AdditiveExpr
                        | ComparisonExpr '>=' AdditiveExpr
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
