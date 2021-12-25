grammar Textaverse;

file : sentence ((SEMI|FULLSTOP) sentence)* FULLSTOP* EOF;

sentence : command (COMMA THEN command)*;
command : (predicate indirectobject |
           predicate indirectobject PREPOSITION object | 
           predicate quotedarg |
           predicate PREPOSITION indirectobject quotedarg |
           predicate quotedarg PREPOSITION indirectobject) (adverb)?;

predicate : verb;
indirectobject : adjectivatedNoun;
object : adjectivatedNoun (AND adjectivatedNoun)*;
//adjectivatedNoun : (adjective)? noun;
adjectivatedNoun : noun;
quotedarg : ANYWORDQUOTED;

verb :  WORD;
adverb : WORD;
//adjective : WORD;
noun : WORD;

LINECOMMENT: '#' ~[\r\n]* -> channel(2);

PREPOSITION : 'in' | 'at' | 'on' | 'of' | 'to' | 'with' | 'from' | 'into';
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