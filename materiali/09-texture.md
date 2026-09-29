# Lezione 9 — Texture e memoria: compressione e gestione degli asset

## Il budget
Il Quest 3S ha 8 GB di RAM condivisi tra sistema, app e GPU: la tua app deve stare sotto 2–2,5 GB in tutto, e le texture sotto ~500 MB. Una sola texture 4K non compressa senza mipmap occupa 64 MB; dieci sono 640 MB, e la banda per leggerle costa frame time.

## Formati
Su Quest si usa **ASTC**: 4x4 per texture in primo piano con dettaglio fine (1 byte/pixel), **6x6** come standard (0,44 byte/pixel), 8x8 per ciò che si vede da lontano (0,25). Mai RGBA32 (non compresso), mai ETC1. Le normal map restano di tipo Normal Map (Unity sceglie il canale giusto).

## Mipmap
Senza mipmap una texture lontana viene campionata "a caso": rumore, shimmering, e tutta la texture in banda. Con mipmap la GPU usa una versione ridotta: più bella e più veloce. Sempre attive, tranne per UI e sprite.

## Texel density
Quanti pixel di texture per metro di superficie: ~512 px/m per ciò che si guarda da vicino (tavoli, oggetti in mano), ~256 px/m per muri e pavimenti, ~128 px/m per lo sfondo. Dimensione = densità × lato dell'oggetto, arrotondata alla potenza di 2. Un muro di 6 m a 256 px/m vuole 1536 → 2048 con tiling, non 4096 senza.

## Gli strumenti
- **RUFA ▸ Strumenti ▸ Texture: audit (selezione)**: seleziona texture o cartelle; in Console una tabella con formato Android, mipmap e memoria stimata, più il totale. Copiala nel report.
- A mano: seleziona le texture; nell'Inspector, scheda **Default**: Max Size 2048 (o 1024) e mipmap accese (**Generate Mipmap**, tra le opzioni avanzate); scheda **Android** (l'icona del robot): **Override For Android**, stessa Max Size, Format **ASTC 6x6**; poi **Apply**.
- **RUFA ▸ Strumenti ▸ Texture: preset mobile 2048 / 1024 (selezione)** fa lo stesso su tutte le texture selezionate.
- Se un modello ha le texture "dentro" (embedded), nell'importer del modello usa **Extract Textures**, altrimenti non puoi impostarle.

## Dove leggere i numeri
Profiler → Memory (Textures, sul visore con la build di profiling), Memory Profiler (package), `adb shell dumpsys meminfo <identifier>` (TOTAL PSS), e la dimensione dell'APK.

## Consegna
Audit prima/dopo delle texture del tuo sandbox (tabella con il totale), dimensione dell'APK prima/dopo, e una riga di motivazione per ogni texture che hai tenuto a 2048 o più.
