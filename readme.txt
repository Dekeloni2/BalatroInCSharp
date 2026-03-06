Dekelatro is a roguelike deck building game. This game has many layers
of complexity so it is recommended to have this readme file open alongside
the game.

Your goal is to face "blinds" that are your enemies, and you must defeat them by
building poker hands.

All poker hands are available
You are allowed to play a straight using an Ace

List of poker hands can be seen here:
https://balatrowiki.org/w/Poker_Hands
Hand chips and mult are different in Dekelatro and are scaled
much higher.

Winning 3 blinds increases the blind size by 1.5x. This is called "ante"

The game has playability up to 7 antes.

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

If you didn't mange to reach the required score and have 0 hands, you lose.

All values for hands, discards, money, reroll costs and much more in the GameController.cs file.
All values for Jokers are in the Joker.cs file

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
- Gros Michel
+15 Mult. 1 out of 6 chance of breaking

- Even Steven
+4 Mult per Even Card played

- Odd Todd
+31 Chips per Odd Card played

- Mystic Summit
+15 Mult if you have no discards remaining

- Misprint
Randomly gain 1-20 mult

- Mask
+20 mult when hand DOES NOT contain a face card

- Fibbonaci
+8 Mult per Ace, 2, 3, 5 or 8 played

- Pi Man
Gain ^3.14 chips when an Ace, 3 and 4 are played

- Jolly Jokers
Gain +8 mult if hand contains a pair

- Zany Joker
Gain +12 mult when hand contains Three of a Kind

- Mad Joker
Gain +10 mult when hand contains a Two Pair

- Crazy Joker
Gain +12 mult when hand contains a Straight

- Constelation
When using a Planet, this joker gains +0.1x (starts at 1x).

- Void Portal (doesn't function)
Every 3rd discard transforms the discarded cards into random cards of the same suit. 

Planet cards:
Planet cards level up the values for every poker hand.

