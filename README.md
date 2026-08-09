# Vroom-Boom
Materia: Lenguajes de Programacion | Periodo: 2026-1 | Estado: Completado

## Equipo de trabajo
- [Raúl Alejandro Garay Vinueza](https://github.com/raulgaray26)
- [Daniel Sebastián Gómez Villafuerte](https://github.com/danielgomez-spec)

## Capturas / Demo
> Video demo

https://github.com/user-attachments/assets/86598fe9-b643-4221-8579-bb7badf00fcf

## Funcionalidad
- [x] Movimiento y control: desplazamiento del vehículo con WASD (cambio de carril y aceleración/frenado), con clamp para mantenerlo dentro del área visible de la cámara. [Commit](https://github.com/raulgaray26/Vroom-Boom/commit/9264036bdc29469202e015a9f8876f6fb69a8663)
- [x] Sistema de combustible: reemplaza el sistema tradicional de vidas; desciende constantemente y termina la partida (Game Over) al llegar a cero. [Commit](https://github.com/raulgaray26/Vroom-Boom/commit/cb7eef78b1c780865a4cf1b0eb6c7c9dd3b1acf7)
- [x] Coleccionables: bidones de combustible (rellenan la barra y suman puntos) y monedas (solo puntaje). [Commit](https://github.com/raulgaray26/Vroom-Boom/commit/cb7eef78b1c780865a4cf1b0eb6c7c9dd3b1acf7)
- [x] Obstáculos menores y letales: conos, vehículos detenidos o barriles normales causan pérdida leve de combustible; los barriles explosivos vacían el combustible por completo y terminan la partida. [Commit](https://github.com/raulgaray26/Vroom-Boom/commit/9a28e4339fe38fdd5a6fb775892c8d2c0b99b1e6)
- [x] Progresión de dificultad y niveles: aumento gradual de velocidad y frecuencia de spawn con el tiempo; dos niveles (carretera de asfalto y carretera de arena-tierra). [Commit](https://github.com/raulgaray26/Vroom-Boom/commit/f613136b1b37a82df6d19b2d74aa693176c9a1c6)
- [x] HUD, pantallas y audio: barra de combustible y puntaje en tiempo real, pantallas de inicio/victoria/derrota con opción de reinicio, música de fondo en bucle y SFX para recolecciones, choques, explosiones y transiciones. [Commit](https://github.com/raulgaray26/Vroom-Boom/commit/a15f3dee3b48abe6107dc64ddf275fe320100feb)

## Tecnologías
`C#` | `Unity (2D)`

## Ejecución
### Opción 1: Jugar el ejecutable (recomendado)
```bash
git clone https://github.com/raulgaray26/Vroom-Boom.git
cd Vroom-Boom
```
Luego abre la carpeta del repositorio y ejecuta el archivo `.exe` incluido en VroomBoomBuild.

### Opción 2: Abrir el proyecto en Unity
```bash
git clone https://github.com/raulgaray26/Vroom-Boom.git
cd Vroom-Boom
```
Abre la carpeta del proyecto desde Unity Hub (misma versión de Unity usada en el proyecto) y ejecuta la escena principal desde el editor.

**Controles:** WASD (movimiento horizontal para cambiar de carril, vertical para acelerar/frenar).

## Métricas de Progreso

| Indicador | Valor |
|---|---|
| Commits totales | 32 |
| Pull Request Merges | 7 |
| Cobertura de pruebas | N/A (no hay pruebas automatizadas) |
| Última actualización | 2026-08-09 |

## Reflexión y Aprendizajes
- **Habilidades desarrolladas:** Dominio de C# aplicado a Unity 2D, implementación de físicas de movimiento (límites espaciales o *clamp*), gestión de interfaces de usuario, y compilación de ejecutables.
- **Qué funcionó bien:** La mecánica de supervivencia basada en el consumo constante de combustible en lugar de vidas, lo que generó un bucle de juego dinámico junto con el efecto visual de "carretera infinita".
- **Qué se podría mejorar:** Mejores sprites, mas niveles y sonidos que añadan realismo.
- **Conceptos clave aplicados de la materia:** Paradigma de programación orientada a objetos en C#, control de flujo lógico para la gestión de estados (Inicio, Victoria, Derrota), y manejo eficiente de instancias para el sistema de *spawn* de coleccionables y obstáculos.
