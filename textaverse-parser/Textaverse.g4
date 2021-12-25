grammar Textaverse;

file : command (SEMI SPACE command)* FULLSTOP* EOF;

command : basiccommand | basiccommand SPACE+ ADVERB | command COMMA SPACE+ THEN SPACE+ command;
basiccommand : predicate SPACE+ object |
               predicate SPACE+ indirectobject SPACE+ object | 
               predicate SPACE+ object SPACE+ PREPOSITION SPACE+ indirectobject | 
               predicate SPACE+ quotedarg;
predicate : VERB;
indirectobject : object;
object : ADJECTIVE SPACE+ NOUN | NOUN | object SPACE+ AND SPACE+ object;
quotedarg : '"' quotedelement '"';
quotedelement : ANYWORD (' ' ANYWORD)*;

COMMA: ',';
ADVERB : 'quickly' | 'slowly';
PREPOSITION : 'in' | 'at' | 'on' | 'of' | 'to' | 'with' | 'from';
VERB :  'attack' | 'drink' | 'shout' | 'say';
ADJECTIVE : 'strong';
NOUN : 'monster' | 'beast' | 'human' | 'orc' | 'axe' | 'sword' | 'water' | 'well';
SEMI : ';';
AND : 'and';
THEN: 'then';
SPACE: ' ';
FULLSTOP: '.';
fragment PUNCT: '!' | '?' | '-';
fragment LOWERCASE  : [a-z] ;
fragment UPPERCASE  : [A-Z] ;
ANYWORD : (LOWERCASE | UPPERCASE | PUNCT)+;