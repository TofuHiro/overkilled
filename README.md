# Overkilled - A private learning project

Overkilled is a small project I created with the purpose of learning how to create multiplayer game. The game is inspired by a game called Overcooked where Overkilled shares similar game mechanics with the addition of combat.

## Infrastructure

The project initially was built using Unity 2022, where the project was later upgraded to Unity 6 for network and overall performance. As Netcode for GameObjects (NGO) was used to implement multiplayer, upgrading to Unity 6 allowed the possibility to upgrade NGO from version 1.9.1 to 2.4.0 providing significant network performance. The newer version of Unity also provided a robust way to implement an network object pooler which is critical for performance in a game that utilizes, creates and destroys alot of items within a short period. 

The project also uses Unity's Lobby and Relay services to allow for peer-to-peer connections for players to create and play in lobbies privately or publically

## What I Learnt

After creating this project, I have built a solid foundation for how multiplayer games function, methods to optimize bandwidth usage, and object authority and ownership.

## Notes

The game is in it's prototype stage where temporary assets were used to visualize playing the game. The project was heavily focused on learning netcode and multiplayer.
