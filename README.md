# 2026_07_03_upm_micro_fish_tbio_server

Aquarium of Micro Fish to demonstrate the TBIO concept.

**TBIO** stands for **Text Byte In Out**.

Consider a client-server architecture:

* The server keeps connected players informed.
* The players send input back to the server.

Using RSA validation, it is possible to identify players and organize tournaments.

This project is part of a demonstration showing how to use Mirror for tournament-style games in Unity.

While coding the Mirror part for my students, I came across this interesting video.

It is close to the Robotarium concept and also to an idea I had in mind a few months ago.

[<img width="849" height="441" alt="image" src="https://github.com/user-attachments/assets/016f233e-f298-48bd-852f-fb75b7d350a3" />](https://github.com/EloiStree/2026_03_23_doc_micro_bit_sensor/issues/309)

* https://github.com/EloiStree/2026_03_23_doc_micro_bit_sensor/issues/309
* https://youtu.be/M733JzO3LBE?t=340

The idea of this project is to showcase how a multiplayer game server can be built independently from the networking layer.

You can think of the networking layer as being similar to a WebSocket backend that guarantees an RSA signature and maintains the link between an integer identifier and an RSA identity.

Let's demonstrate how this works.
