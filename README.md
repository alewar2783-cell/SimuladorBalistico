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

### 4. Registro del resultado
* Durante el vuelo del proyectil, se muestra telemetría en vivo (velocidad actual y coordenadas espaciales).
* Al impactar, el tiempo se ralentiza (Slow-Motion) para observar las colisiones. Luego de 3 segundos, se despliega el **Reporte de Tiro**.
* **Datos guardados y mostrados:** 
  * Tiempo de vuelo
  * Punto exacto de impacto (Coordenadas X, Y, Z)
  * Velocidad relativa al momento del choque
  * Impulso de colisión
  * Piezas/Joints derribados (Puntuación)
* Los datos pueden ser exportados a un archivo CSV local para su evaluación mediante el botón **Exportar Datos**.

---

## 🎮 Cómo Jugar

1. **Abrir la escena:** Abre `Assets/_Project/Scenes/SimuladorBalistica.unity`.
2. **Setup de Escena (Solo Editor):** Si la escena está vacía, ve al menú superior de Unity y haz clic en **BallisticSim -> Setup Full Scene**. Esto construirá toda la arquitectura, jerarquía, modelos, cámaras y UI automáticamente.
3. **Play:** Presiona Play en Unity.
4. **Configurar:** Ajusta los sliders del arma (arriba) y del muro objetivo (abajo). Recomendamos probar el tiro inicial por defecto que apunta directo al centro del muro.
5. **Disparar:** Haz clic en **FIRE**. La cámara Cinemachine seguirá el proyectil de cerca.
6. **Resolución:** Observa el impacto en cámara lenta. Analiza el puntaje y presiona **CLEAN SCENE** para volver a intentar con distintos parámetros.

## 📂 Estructura de Assets y Arquitectura (MVC)
El código está separado en 3 capas puras ubicadas en `Assets/_Project/Scripts/`:
* `Model/`: Clases serializables con datos puros, sin dependencias de Unity UI.
* `View/`: Scripts encargados puramente de actualizar el TextMeshPro y los Sliders.
* `Controller/`: El cerebro. Suscribe eventos, lanza el proyectil y orquesta las físicas.
* `Camera/`, `Spawner/`, `Editor/`, `Projectile/`: Sistemas modulares aislados.

## 📝 Criterios de Evaluación y Commits
El repositorio de Git mantiene un historial claro de commits bajo el formato *Conventional Commits* (ej. `feat: ...`, `fix: ...`), dividiendo el desarrollo lógico desde la arquitectura base hasta la implementación de Cinemachine y físicas avanzadas.