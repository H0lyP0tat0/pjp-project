grammar MFL;

prog: statement* EOF;

expr: '-' expr
    | '!' expr 
    | expr ('*'|'/'|'%') expr
    | expr ('+'|'-'|'.') expr
    | expr ('<'|'>') expr
    | expr ('=='|'!=') expr
    | expr '&&' expr
    | expr '||' expr
    | ID '=' expr
    | ID
    | INT                   
    | FLOAT                 
    | BOOL                  
    | STRING                
    | '(' expr ')'          
    ;

INT : [1-9][0-9]* | [0] ;
FLOAT : [0-9]+[.][0-9]* ;
BOOL : 'true' | 'false' ;
STRING : '"' CHARSEQUENCE* '"' ;
ID : [a-zA-Z] [a-zA-Z0-9]* ;
WS : [ \t\r\n]+ -> skip ;
COMMENT : '//' ~[\r\n]* -> skip ;

// types
type : 'int' | 'string' | 'float' | 'bool' ;

// statements
statement
    : type ID (',' ID)* ';'
    | expr ';'
    | 'read' ID (',' ID)* ';'
    | 'write' expr (',' expr)* ';'
    | '{' statement+ '}'
    | 'if' '(' expr ')' statement ('else' statement)?
    | 'while' '(' expr ')' statement
    | ';'
    ;
    
fragment CHARSEQUENCE
    : ~["\\\r\n]
    | '\\' ['"?abfnrtv\\]
    | '\\\n'
    | '\\\r\n'
    ;
