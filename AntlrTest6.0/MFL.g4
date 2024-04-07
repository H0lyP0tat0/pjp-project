grammar MFL;

prog: statement* EOF;

expr: '-' expr                      # unMinus
    | '!' expr                      # not
    | expr op=('*'|'/'|'%') expr    # mulDivMod   
    | expr op=('+'|'-'|'.') expr    # addSubCon
    | expr op=('<'|'>') expr        # lessGreater
    | expr op=('=='|'!=') expr      # equalNotEqual
    | expr '&&' expr                # and
    | expr '||' expr                # or
    | ID '=' expr                   # assign
    | ID                            # id
    | INT                           # int
    | FLOAT                         # float
    | BOOL                          # bool
    | STRING                        # string
    | '(' expr ')'                  # brackets
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
    : type ID (',' ID)* ';'                             # declaration
    | expr ';'                                          # expression
    | 'read' ID (',' ID)* ';'                           # read
    | 'write' expr (',' expr)* ';'                      # write
    | '{' statement+ '}'                                # block
    | 'if' '(' expr ')' statement ('else' statement)?   # if
    | 'while' '(' expr ')' statement                    # while
    | ';'                                               # empty
    ;
    
fragment CHARSEQUENCE
    : ~["\\\r\n]
    | '\\' ['"?abfnrtv\\]
    | '\\\n'
    | '\\\r\n'
    ;
