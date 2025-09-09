# OWOChat

**OWOChat** es una aplicación de videollamadas desarrollada en **Unity**, que combina **.NET** para la gestión de mensajes de texto a través de **TCP**, y transmisión de video en tiempo real mediante **UDP**. Este proyecto busca ofrecer un sistema de comunicación simple y eficiente que aprovecha diferentes protocolos de red para optimizar la experiencia.

---

## 🚀 Características principales

* **Videollamadas en tiempo real** usando **UDP** para menor latencia.
* **Chat de texto confiable** implementado sobre **TCP**.
* Interfaz desarrollada en **Unity** para una experiencia interactiva y extensible.
* Soporte para conexiones cliente-servidor.
* Arquitectura modular que permite añadir nuevas funciones en el futuro.

---

## 🛠️ Tecnologías utilizadas

* [Unity](https://unity.com/) (Motor principal)
* [.NET / C#](https://dotnet.microsoft.com/)
* **TCP** → comunicación confiable para mensajes de texto.
* **UDP** → transmisión rápida de video.

---



## 📡 Arquitectura de red

* **Servidor TCP:** maneja los mensajes de texto y asegura su entrega.
* **Servidor UDP:** transmite paquetes de video para reducir la latencia.
* **Cliente:** envía y recibe datos a través de ambos protocolos simultáneamente.

---
