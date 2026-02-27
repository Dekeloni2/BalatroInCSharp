Dekelatro is a roguelike deck building game. This game has many layers
of complexity so it is recommended to have this readme file open alongside
the game.

Your goal is to face "blinds" that are your enemies, and you must defeat them by
building poker hands.

All poker hands are available, and some special ones are included (Five of a Kind, Flush Five, Flush House)
You are allowed to play a straight using an Ace

List of poker hands can be seen here:
https://balatrowiki.org/w/Poker_Hands
Hand chips and mult are different in Dekelatro and are scaled
much higher.

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

X - View consumables
This lets you see all consumable types that are
currently in your inventory

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

Planet cards:
Planet cards level up the values for every poker hand.