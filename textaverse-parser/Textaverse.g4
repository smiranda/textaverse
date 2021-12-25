grammar Textaverse;

file : command (SEMI command)* FULLSTOP* EOF;

command : basiccommand | basiccommand ADVERB | command COMMA THEN command;
basiccommand : predicate object |
               predicate indirectobject PREPOSITION object | 
               predicate quotedarg;
predicate : VERB;
indirectobject : object;
object : ADJECTIVE NOUN | NOUN | object AND object;
quotedarg : '"' quotedelement '"';
quotedelement : ANYWORD (ANYWORD)*;

COMMA: ',';
ADVERB : 'quickly' | 'slowly';
PREPOSITION : 'in' | 'at' | 'on' | 'of' | 'to' | 'with' | 'from';
VERB :  'attack' | 'drink' | 'shout' | 'say';
ADJECTIVE : 'strong';
NOUN : 'monster' | 'beast' | 'human' | 'orc' | 'axe' | 'sword' | 'water' | 'well';
DETDEF: 'the';
SEMI : ';';
AND : 'and';
THEN: 'then';
FULLSTOP: '.';
fragment PUNCT: '!' | '?' | '-';
fragment LOWERCASE  : [a-z] ;
fragment UPPERCASE  : [A-Z] ;
ANYWORD : (LOWERCASE | UPPERCASE | PUNCT)+;
CR : '\r' -> skip ;
NEWLINES : '\n' -> skip ;
WHITESPACE : ' ' -> skip ;