# 🎯 Simulador Balístico Paramétrico

**Versión de Unity:** 6.3  
**Arquitectura:** MVC (Model-View-Controller) Estricto  

## 🚀 Descripción General

Este proyecto es un simulador balístico interactivo desarrollado en Unity 6.3 que cumple estrictamente con los requisitos de simulación física mediante `Rigidbody` y `Joints`. Permite al usuario configurar variables físicas precisas para disparar un proyectil contra estructuras paramétricas y medir los resultados del impacto.

---

## ✅ Cumplimiento de la Consigna (Requisitos Mínimos)

### 1. Controles de disparo en pantalla
* El simulador incluye una interfaz de usuario completa (Canvas) dividida en dos paneles interactivos.
* **Ángulo, fuerza y masa del proyectil:** Se configuran en tiempo real mediante *Sliders* ubicados en el panel superior izquierdo. Además, se permite configurar el tamaño y la capacidad de rebote del proyectil.

### 2. Disparo físico
* **Proyectil:** Es un GameObject instanciado dinámicamente que cuenta con un `SphereCollider` y un `Rigidbody`.
* **Lanzamiento:** Utiliza el sistema de físicas nativo. Al disparar, se calcula la rotación del cañón en base al ángulo configurado y se aplica fuerza usando `Rigidbody.AddForce(direction, ForceMode.Impulse)`. El `Collision Detection` está seteado en `Continuous Dynamic` para evitar *tunneling*.

### 3. Escena de objetivos
* **Estructuras:** Se genera un muro paramétrico de cajas. El usuario puede elegir las filas, columnas, masa y tamaño desde el panel inferior.
* **Joints:** Las cajas se conectan estructuralmente mediante `FixedJoint`. La fuerza de ruptura (*Break Force*) es totalmente personalizable.
* **Estabilidad:** La cuadrícula se genera con precisión matemática, espaciando los bloques exactamente según su escala (0.0mm de overlap) para garantizar una estabilidad estructural perfecta sin repulsión inicial.

### 4. Registro y Persistencia en la Nube (Fase 3 UGS)
* Durante el vuelo del proyectil, se muestra telemetría en vivo (velocidad actual y coordenadas espaciales).
* Al impactar, el tiempo se ralentiza (Slow-Motion) para observar las colisiones. Luego de 3 segundos, se despliega el **Reporte de Tiro** en el borde derecho de la pantalla.
* **Datos calculados:** Tiempo de vuelo, punto de impacto, velocidad relativa, impulso, piezas derribadas.
* **Persistencia Local y Nube (Auto-guardado):** 
  * Se puede **Exportar a CSV** localmente usando el botón correspondiente.
  * **Patrón Repository y Unity Gaming Services (UGS):** Al finalizar cada disparo, el controlador evalúa el impacto y sube automáticamente a la nube un paquete DTO (`SimulationRecord`) que incluye *Ángulo, Fuerza, Masa, Distancia, si hubo acierto o no, y la cantidad de objetos afectados*. Todo esto se gestiona mediante el `UgsSimulationRepository`.
  * **Historial de Tiros:** El sistema en la nube almacena un array histórico con los últimos 10 disparos realizados. En cualquier momento se puede presionar el botón **SHOW HISTORY** (en el panel inferior derecho) para descargar de UGS la lista formateada y desplegarla en un panel central para comparar resultados.

### 5. Experiencia de Usuario y Cámaras Inteligentes
* **Cámaras Contextuales:** Al ajustar los sliders del cañón, la cámara muestra una vista general. Al configurar el muro (columnas, filas, distancia), el sistema de Cinemachine transiciona automáticamente a una `TargetCamera` que sigue de cerca la posición paramétrica de las cajas.
* **Cinemachine Tracking Avanzado:** Al disparar, el `CinemachineBrain` hace un corte cinemático instantáneo (`Cut`) hacia una `TrackingCamera` optimizada sin retrasos (Damping 0) con un apuntado duro (`HardLookAt`) para seguir perfectamente al proyectil de alta velocidad, finalizando en una cámara fija dedicada para observar la reacción en cadena del impacto.

---

## 🎮 Cómo Jugar

1. **Abrir la escena:** Abre `Assets/_Project/Scenes/SimuladorBalistica.unity`.
2. **Setup de Escena (Solo Editor):** Si la escena está vacía, ve al menú superior de Unity y haz clic en **BallisticSim -> Setup Full Scene**. Esto construirá toda la arquitectura, jerarquía, modelos, cámaras y UI automáticamente.
3. **Play:** Presiona Play en Unity. (Asegúrate de estar logueado y tener el proyecto vinculado en `Edit -> Project Settings -> Services` para que funcione el guardado en la nube).
4. **Configurar:** Ajusta los sliders del arma (arriba) y del muro objetivo (abajo). Recomendamos probar el tiro inicial por defecto que apunta directo al centro del muro. La cámara cambiará dinámicamente según lo que estés ajustando.
5. **Disparar:** Haz clic en **FIRE** (o usa la barra espaciadora). La cámara seguirá al proyectil.
6. **Resolución y Guardado Automático:** Observa el impacto. Analiza el reporte de tiro. El sistema registrará el tiro en tu historial de la nube automáticamente.
7. **Historial:** Presiona **CLEAN SCENE** para resetear la pared. Luego, haz clic en **SHOW HISTORY** (abajo a la derecha) para descargar y ver tu lista de tiros desde UGS.

## 📂 Estructura de Assets y Arquitectura (MVC)
El código está separado en 3 capas puras ubicadas en `Assets/_Project/Scripts/`:
* `Model/`: Clases serializables con datos puros y Data Transfer Objects (DTOs).
* `View/`: Scripts encargados puramente de actualizar el TextMeshPro y los Sliders.
* `Controller/`: El cerebro. Suscribe eventos, lanza el proyectil y orquesta las físicas.
* `Repository/`: Implementación del Patrón de Inversión de Dependencias (Repository Pattern) con integraciones a **Unity Gaming Services**.
* `Camera/`, `Spawner/`, `Editor/`, `Services/`: Sistemas modulares y servicios asincrónicos.

## 📝 Criterios de Evaluación y Commits
El repositorio de Git mantiene un historial claro de commits bajo el formato *Conventional Commits* (ej. `feat: ...`, `fix: ...`), dividiendo el desarrollo lógico desde la arquitectura base MVC, simulador físico, persistencia en la nube (Fase 3), y refinamiento UX/UI.