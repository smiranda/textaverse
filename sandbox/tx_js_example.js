hasNames("orb", "shiny orb", "obsidian orb","shiny obsidian orb", "misterious orb");
hasId("tx:object:magic:obsidian-orb");
hasBootLocation("tx:room:hall");
hasDescription("A shiny obsidian orb which emits a misterious glow.");
hasDescription("The orb is engraved in a stone pedestal.",
			   (self)=> self.isAt("tx:room:hall"));
hasDescription("The orb is red-hot from absorbing the hot fumes.", 
			   (self)=> self.isAt("tx:room:volcano:summit"));
hasDescription("The orb is emitting an eery sound.", 
			   (self)=>
					self.owner().isAgent() && (
   					self.owner().hasObject("tx:object:magic:wizard-hat") ||
					self.owner().hasObject("tx:object:magic:wizard-staff")));
hasDescription("The sound is a voice telling you 'Take the ivory key to the iron chest in the throne room'.", 
			   (self)=>
					self.owner().isAgent() && (
   					self.owner().hasObject("tx:object:magic:wizard-hat") &&
					self.owner().hasObject("tx:object:magic:wizard-staff")));

hasTrait("float"); // E.g., if thrown into water ?
hasTrait("burn"); // E.g., Burns if in contact with fire
hasTrigger("say", (self, text)=> 
	text.indexOf('ivory key') > 0 ? "Take the ivory key to the iron chest in the throne room" : "The orb says nothing in return.";
);

/*

 There's an "orb" here. It's also known as "shiny orb", "obsidian orb","shiny obsidian orb", and "misterious orb".
 
 It's like "A shiny obsidian orb which emits a misterious glow.".
 
 If it's at 'tx:room:hall' then it's like "The orb is engraved in a stone pedestal.".
 
 If it's at 'tx:room:volcano:summit' then it's like "The orb is red-hot from absorbing the hot fumes.".
 
 If owner is someone and (owner has 'tx:object:magic:wizard-hat' or owner has 'tx:object:magic:wizard-staff') then it's like 'The orb is emitting an eery sound.'. 
 
 If owner is someone and owner has 'tx:object:magic:wizard-hat' and owner has 'tx:object:magic:wizard-staff' then it's like 'The sound is a voice telling you 'Take the ivory key to the iron chest in the throne room'. 
 
 It floats and it burns.
 
 If someone says 'ivory key' then say 'Take the ivory key to the iron chest in the throne room' else say 'They orb says nothing in return'.
 
*/