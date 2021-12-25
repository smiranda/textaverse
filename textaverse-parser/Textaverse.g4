grammar Textaverse;

file : command ((SEMI|FULLSTOP) command)* FULLSTOP* EOF;

command : basiccommand | basiccommand ADVERB | command COMMA THEN command;
basiccommand : predicate object |
               predicate indirectobject PREPOSITION object | 
               predicate quotedarg;
predicate : VERB;
indirectobject : object;
object : ADJECTIVE NOUN | NOUN | object AND object;
quotedarg : ANYWORDQUOTED;

ADVERB : 'quickly' | 'slowly';
PREPOSITION : 'in' | 'at' | 'on' | 'of' | 'to' | 'with' | 'from';
VERB :  'attack' | 'drink' | 'shout' | 'say';
ADJECTIVE : 'strong';
NOUN : 'monster' | 'beast' | 'human' | 'orc' | 'axe' | 'sword' | 'water' | 'well';
DETDEF: 'the';
AND : 'and';
THEN: 'then';
fragment PUNCT: '!' | '?' | '-' | ',' | '.' | ' ';
fragment LOWERCASE  : [a-z] ;
fragment UPPERCASE  : [A-Z] ;
fragment QUOTE : '"' ;
ANYWORDQUOTED : QUOTE (LOWERCASE | UPPERCASE | PUNCT)+ QUOTE;
COMMA: ',';
SEMI : ';';
FULLSTOP: '.';
CR : '\r' -> skip ;
NEWLINES : '\n' -> skip ;
WHITESPACE : ' ' -> skip ;