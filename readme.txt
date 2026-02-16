Dekelatro is a roguelike deck building game. This game has many layers
of complexity so it is recommended to have this readme file open alongside
the game.

Your goal is to face "blinds" that are your enemies, and you must defeat them by
building poker hands.

All poker hands are available, and some special ones are included (Five of a Kind, Flush Five, Flush House)
You are allowed to play a straight using an Ace

List of poker hands can be seen here:
https://balatrowiki.org/w/Poker_Hands

Every time you beat 2 blinds, you face a "boss blind" that contains a special
ability that makes it harder for you to beat it.

Winning 3 blinds increases the blind size by 1.5x. This is called "ante"

The game has playability up to 8 antes.

In blind gameplay:
To play a hand, write indexes for the cards that represent them.
Each card gives you specified "chips". J/Q/K give 11, 12, 13 respectively
and an Ace gives 14.

Each hand has certain values (that are upgradable) when played.

The score is calculated by Chips * Mult.
Example:
Playing a straight with an Ace, Two, Three, Four, Five will give you a total of
14 + 2 + 3 + 4 + 5 = 28
Since that is a straight, it will give you the Straight Bonus
30 (+28) * 4 (Mult) = 232 Score

Controls:

In the blind:
D - Discard cards
If you are not happy with the cards in your hand, you can
write indexes to swap them from your deck

S - Sort by rank
Sorts your hand by rank for easier clearance

A - Sort by suit
Sorts your hand by suit (Club, Spade, Diamond, Hearts) for easier
clearance

C - View cards
This allows you to see what cards are inside your deck.

J - View jokers
This lets you see purchased jokers and lets you reorganize
and sell them

In the shop:

R - Reroll
If you are not happy with what you have in your shop,
you may reroll. Reroll increases by +1$

J - View jokers
This lets you see purchased jokers and lets you reorganize
and sell them

F - Finish
This lets you go to the next blind



Jokers:

Jokers are passive items that increase your score.
Each joker has a different ability, and you can see
what each does by clicking J to view them.

List of available jokers:


Enchancements:

Enchantments are special abilities that are applied
to cards using tarot cards. For the sake of simplicity
and UX, Enchantments are represented using emojis in game.

Enchantments available:
1. Mult:
Gain +5 mult on played card
2. Bonus
Gain +40 chips on played card
3. Wild
Is considered as all suits. Cannot be debuffed by boss blinds
4. Gold
When blind is won, and this card is held in hand
gain +3$
5. Steel
1.5x Mult when held in hand
6. Lucky
1/5 chance of +20 Mult. 1/20 of +20$ at the end of the round.
7. Glass
Card gives 2x. 1/4 chance of card being destroyed


Tarot cards:
Tarot cards are special cards that allow you to enchant
your cards, in the blind or in packs. They can appear in
the shop rotation or 1 of the 2 packs in teh shop.

For the sake of UX, Tarot cards are represented by emojis


Tarot cards:
The Hermit 🧔‍♀️:
Double your money (max of 20$)
The Fool🧝:
Duplicate your last used tarot or planet (cannot duplicate a fool)
The Empress👸
Apply Mult to 2 selected cards
The Priestess 🧕:
Gain 2 random planet cards
Temperence 👴:
Gain +X sell value of your jokers (up to 50$)
Death 🧟:
Write 2 (eg. 0 1) to copy the first card
Emperor 🫅:
Gain 2 random tarot cards
Justice 🧑‍⚖️:
Turn a selected card into glass
Strength 💪:
Increases rank of 2 selected cards by 1