# Lezione 10 — Rendering avanzato per Meta Quest: single pass, MSAA, FFR, render scale, risoluzione dinamica

## Single Pass Instanced
In VR si disegnano due immagini. Con **Multi Pass** tutto viene fatto due volte (draw call doppie). Con **Single Pass Instanced** ogni draw call disegna entrambi gli occhi in una volta. Si imposta in Project Settings → XR Plug-in Management → OpenXR → Render Mode (o con **RUFA ▸ Strumenti ▸ OpenXR: single pass e foveated rendering**). È un'impostazione di build: si confronta con due build. Il sandbox parte in **Multi Pass**: passare a Single Pass Instanced è il primo passo di oggi, e vale sia per la scena del demo sia per le tue.

## MSAA 4x
Su una GPU "tile-based" come quella del Quest il multisampling costa pochissimo ed elimina le scalettature che in VR danno fastidio. Sempre 4x (URP Asset → Anti Aliasing).

## Render scale
I pixel costano al quadrato: render scale 1,5 = 2,25 volte i pixel. Tienilo a 1,0; scendi a 0,8–0,9 solo se serve.

## Fixed Foveated Rendering (FFR)
La periferia dell'immagine viene disegnata a risoluzione più bassa: meno pixel, stessa nitidezza al centro. Attiva la feature **Foveated Rendering** in OpenXR (Android) e imposta il livello da script: il componente **FoveationLevel** del nucleo lo fa all'avvio (0 = off, 0,5 = medio, 1 = alto). Guarda i bordi dello schermo per vedere l'effetto.

## Risoluzione dinamica
Il componente **AdaptiveRenderScale** del nucleo legge il tempo GPU e abbassa il render scale quando supera 10 ms, lo rialza sotto 7,5 ms (limiti 0,7–1,0). Richiede "Frame Timing Stats" attivo (lo fa il setup). È il paracadute contro i picchi, non una scusa per non ottimizzare.

## Cose da spegnere
- **Post-processing** (bloom, vignette, SSAO): una o più passate a schermo pieno per occhio. Su Quest quasi sempre no.
- **HDR**: render target più pesanti e un tone-mapping in più.
- **Depth Texture / Opaque Texture** nell'URP Asset: una copia dello schermo per frame; attivale solo se un effetto le usa davvero.
- Ombre: una cascade, distanza corta, shadow map piccole.

Tutto insieme: **RUFA ▸ Strumenti ▸ URP: preset Quest** sull'asset URP selezionato.

## Misurare
Cambia una cosa alla volta e lancia il benchmark: il CSV riporta render scale e MSAA usati. Nel report: una riga per feature con frame time prima/dopo.

## Consegna
Attivazione e tuning delle feature sul tuo sandbox con la misura dell'impatto di ciascuna, e il tuo blockout a 90 fps stabili sul benchmark.
