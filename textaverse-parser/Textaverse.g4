grammar Textaverse;

file : sentence ((SEMI|FULLSTOP) sentence)* FULLSTOP* EOF;

sentence : command | command adverb | sentence COMMA THEN sentence;
command : predicate object |
          predicate indirectobject PREPOSITION object | 
          predicate quotedarg;
predicate : verb;
indirectobject : object;
object : (adjective)* noun (AND noun)*;
quotedarg : ANYWORDQUOTED;

verb :  WORD;
adverb : WORD;
adjective : WORD;
noun : WORD;

LINECOMMENT: '#' ~[\r\n]* -> channel(2);

PREPOSITION : 'in' | 'at' | 'on' | 'of' | 'to' | 'with' | 'from';
DETDEF: 'the';
AND : 'and';
THEN: 'then';

fragment PUNCT: '!' | '?' | '-' | ',' | '.' | ' ';
fragment LOWERCASE  : [a-z] ;
fragment UPPERCASE  : [A-Z] ;
fragment QUOTE : '"' ;
ANYWORDQUOTED : QUOTE (LOWERCASE | UPPERCASE | PUNCT)+ QUOTE;

WORD: (LOWERCASE | UPPERCASE)+;

COMMA: ',';
SEMI : ';';
FULLSTOP: '.';
CR : '\r' -> skip ;
NEWLINES : '\n' -> skip ;
WHITESPACE : ' ' -> skip ;