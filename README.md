# 🎯 Simulador Balístico Paramétrico

**Versión de Unity:** 6.3  
**Arquitectura:** MVC (Model-View-Controller) Estricto  

## 🚀 Descripción General

Este proyecto es un simulador balístico interactivo desarrollado en Unity 6.3. Permite al usuario configurar variables físicas precisas para disparar un proyectil contra estructuras paramétricas generadas proceduralmente. 

El simulador está diseñado para registrar datos telemétricos en tiempo real y evaluar el impacto físico, calculando la transferencia de energía, daños estructurales y distancias. Al finalizar cada disparo, el usuario recibe un "Reporte de Tiro" detallado con la opción de exportar los resultados para su posterior análisis.

## ✨ Características Principales

*   **Configuración Paramétrica:** Ajuste en tiempo real del ángulo de disparo, fuerza, masa y tamaño del proyectil mediante una interfaz minimalista.
*   **Físicas Avanzadas:** Uso intensivo del sistema de físicas de Unity (`Rigidbody`). Los proyectiles utilizan detección de colisión `Continuous Dynamic` para evitar que la bala atraviese objetos a altas velocidades (*tunneling*)[cite: 1].
*   **Generación Procedural de Objetivos:** Un spawner automático crea muros de cajas conectadas dinámicamente mediante `Joints` (Fixed/Hinge), garantizando estabilidad física antes del impacto.
*   **Cámaras Dinámicas:** Integración de cámara cinemática que transita desde una vista isométrica panorámica (configuración) a una cámara en tercera persona anclada al proyectil durante el vuelo.
*   **Telemetría y Reportes:** Lectura en tiempo real de la velocidad y coordenadas espaciales (X, Y, Z). Al finalizar el impacto, se genera un reporte y un puntaje basado en la cantidad de uniones (*joints*) destruidas.

## 🏗️ Arquitectura del Proyecto (MVC)

El código fuente está estrictamente estructurado bajo el patrón **Model-View-Controller** para garantizar la escalabilidad y limpieza del código[cite: 2]:

*   **Model (`BallisticModel.cs`):** Contiene exclusivamente los datos del simulador (parámetros de entrada como fuerza y masa, y resultados como distancia y tiempo de vuelo) y las reglas de negocio[cite: 2]. No tiene dependencias de la interfaz[cite: 2].
*   **View (`BallisticView.cs`):** Muestra la información al usuario (paneles, textos, Sliders) y captura su interacción[cite: 2]. Es una interfaz limpia, sin reglas de simulación integradas[cite: 2].
*   **Controller (`BallisticController.cs`):** El núcleo lógico. Recibe la interacción del usuario desde la vista, actualiza los datos del modelo y solicita la ejecución de la simulación física a Unity[cite: 2].

## 🎮 Controles y Uso de la Interfaz

1.  **Fase de Preparación:** Al iniciar, la cámara general muestra el cañón y el entorno. El usuario utiliza los Sliders (rango de ángulo, por ejemplo, de 0 a 90 grados[cite: 1]) para definir las variables iniciales y la distancia de la estructura objetivo.
2.  **Disparo:** Al presionar el botón "DISPARAR", el `Controller` instancia el proyectil y le aplica un impulso físico instantáneo (`ForceMode.Impulse`) dictado por los parámetros del `Model`[cite: 1].
3.  **Seguimiento:** La cámara cambia automáticamente para seguir la trayectoria de la bala. En pantalla se muestra la telemetría en vivo.
4.  **Resolución:** Al impactar contra el muro objetivo, el motor de físicas calcula las colisiones. Luego de unos segundos, la interfaz muestra el "Reporte de Tiro".

## 📊 Exportación de Datos

El simulador cuenta con una herramienta nativa para investigación y análisis de datos. Desde el panel de "Reporte de Tiro", el usuario puede hacer clic en **Exportar Datos**. 
Esto genera un archivo local que incluye:
*   Velocidad inicial y masa.
*   Ángulo de lanzamiento.
*   Tiempo total de vuelo.
*   Coordenadas exactas del punto de impacto.
*   Magnitud del impulso y velocidad relativa al momento del choque.
*   Cantidad de cajas derribadas/joints rotos.

## ⚙️ Tecnologías Utilizadas
*   **Motor:** Unity 6.3
*   **Lenguaje:** C#
*   **UI:** Unity Canvas & TextMeshPro
*   **Cámaras:** Unity Cinemachine / Splines