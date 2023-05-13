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
                        | ReturnStmt
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

ReturnStmt              : 'return' ExpressionList ';'? NEWLINE
                        ;

IdentifierList          : Identifier (',' Identifier)*
                        ;

ExpressionList          : Expression (',' Expression)*
                        ;

Type                    : TYPE_KEYWORD
                        | FuncType
                        ;

FuncType                : 'func' '(' TypeList ')' ('(' TypeList ')' | Type)?
                        ;

TypeList                : Type (',' Type)*
                        ;

Expression              : FuncExpr
                        | LogicalOrExpr
                        ;

FuncExpr                : 'func' FuncSignature
                        ;

FuncSignature           : '(' ParamList? ')' ('(' TypeList ')' | Type)? Compound
                        ;

ParamList               : (IDENTIFIER Type? ',')? IDENTIFIER Type
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
                        : CallExpr
                        ;

CallExpr                : CallExpr '(' ExpressionList ')'
                        : PrimaryExpr

PrimaryExpr             : IdExpr
                        | INTEGER_LITERAL
                        | BOOL_LITERAL
                        | '(' Expression ')'
                        ;
```
