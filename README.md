
```init
git submodule add https://github.com/EloiStree/2026_07_03_upm_micro_fish_tbio_server.git Packages/be.elab.tbiomicrofish
```

# 2026_07_03_upm_micro_fish_tbio_server

Aquarium of Micro Fish to demonstrate the TBIO concept.

<img width="1697" height="906" alt="image" src="https://github.com/user-attachments/assets/1dde56be-1445-4238-bb32-f2d9583787e3" />     
<img width="1694" height="950" alt="image" src="https://github.com/user-attachments/assets/cf879d8f-cc64-4395-8b11-34f166bb1f99" />   
<img width="534" height="412" alt="image" src="https://github.com/user-attachments/assets/c83f62f2-835a-4748-9143-2e5ad9aad0f9" />   


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


**See:**
- 2026_06_29_upm_text_byte_in_out_layer_mirror_rsa
- 2026_06_29_upm_text_byte_in_out_layer
