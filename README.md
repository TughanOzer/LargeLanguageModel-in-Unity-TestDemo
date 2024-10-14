Testing how to implement a LLM into Unity, and trying out different use-cases.
This is just a rough demo!
This demo so far has a chat feature with history, and a character, which is controlled by the LLM (so far only random movements, until Img2Txt is implemented)

![MenuPresentation](https://github.com/user-attachments/assets/c7ca1943-d3c6-4fbd-8eaf-12f45d8864e2)

Setup:
1. Open this project in Unity 2022
2. Install and run KoboldCPP https://github.com/LostRuins/koboldcpp/releases
3. Install and load LLM model into Kobold (expl. Meta-Llama-3.1-8B-Instruct-Q6_K)
4. Make sure Kobold runs on port 5001, or alternatively change the port in the script.
5. The System Prompt can be adjusted under Resources/SytemPrompt.json


Future plans:
- Implementing Image-2-Text, to give the Ai a visual description of its environment.
- Trying other simple gamified use-cases (tic tac toe, rock paper scissors or some form of coin collection)
