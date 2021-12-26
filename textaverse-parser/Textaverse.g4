grammar Textaverse;

file: sentence ((SEMI | FULLSTOP) sentence)* FULLSTOP* EOF;

sentence: command (COMMA THEN command)*;
command: (
		predicate indirectobject
		| predicate indirectobject PREPOSITION object
		| predicate quotedarg
		| predicate PREPOSITION indirectobject quotedarg
		| predicate quotedarg PREPOSITION indirectobject
	) (adverb)?;

predicate: verb;
indirectobject: (DETERM)? adjectivatedNoun;
object: (DETERM)? adjectivatedNoun (AND adjectivatedNoun)*;
//adjectivatedNoun : (adjective)? noun;
adjectivatedNoun: noun;
quotedarg: ANYWORDQUOTED;

verb: WORD;
adverb: WORD;
//adjective : WORD;
noun: WORD;

LINECOMMENT: '#' ~[\r\n]* -> channel(2);

PREPOSITION:
	'in'
	| 'at'
	| 'on'
	| 'of'
	| 'to'
	| 'with'
	| 'from'
	| 'into';
DETERM: 'the' | 'a' | 'an';
AND: 'and';
THEN: 'then';

fragment PUNCT:
	'!'
	| '?'
	| ','
	| '.'
	| ' '
	| '('
	| ')'
	| '['
	| ']'
	| '{'
	| '}'
	| ';'
	| '='
	| '+'
	| '-'
	| '*'
	| '\''
	| '/'
	| '<'
	| '>';
fragment LOWERCASE: [a-z];
fragment UPPERCASE: [A-Z];
fragment NUM: [0-9];
fragment QUOTE: '"';
ANYWORDQUOTED:
	QUOTE (LOWERCASE | UPPERCASE | PUNCT | NUM)+ QUOTE;

WORD: (LOWERCASE | UPPERCASE | '-' | '_' | NUM)+;

COMMA: ',';
SEMI: ';';
FULLSTOP: '.';
CR: '\r' -> skip;
NEWLINES: '\n' -> skip;
WHITESPACE: ' ' -> skip;