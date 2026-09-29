# Relazione tecnica di ottimizzazione — template (lezione 15, esame)

**Titolo del progetto** · **Genere** (walking simulator / escape room) · **Autore** · **Data**

## 1. Concept approvato e cosa è stato realizzato
Tre righe sul concept (come approvato alla lezione 4) e tre su cosa c'è nella build finale.

## 2. Piattaforma e build
Unity 6000.3.25f1, URP, XR Interaction Toolkit, OpenXR. Build Quest 3S (APK) e Windows. Versione. Percorso di benchmark: descrizione delle stazioni (dove sono, cosa guardano) con uno screenshot dall'alto.

## 3. Misure prima/dopo
Tutte con build non development, 90 Hz, stesso percorso. "Prima" = lezione 5 (blockout). Le colonne "metrica" sono quelle della lezione.

| Lezione | Tecnica applicata | Metrica | Prima | Dopo | Note (cosa hai fatto esattamente) |
|---|---|---|---|---|---|
| 6 | LOD, static flags, occlusion culling | Triangoli a schermo, GPU ms | | | |
| 7 | Materiali condivisi, atlas, SRP Batcher, instancing, Simple Lit | Draw call, SetPass, overdraw | | | |
| 8 | Lighting baked, probe, reflection probe | GPU ms, luci realtime attive | | | |
| 9 | ASTC, mipmap, texel density | Memoria texture (MB), dimensione APK | | | |
| 10 | Single pass, MSAA, FFR, render scale, risoluzione dinamica | Frame time ms, fps | | | |
| 11 | Particellari ottimizzati | Overdraw, GPU ms | | | |
| 12 | Audio, haptics, comfort | Checklist di comfort (n. Sì / 20) | | | |

Riga finale: frame time medio e 1 % low sulle stazioni (dal CSV) alla lezione 5 e alla lezione 15.

## 4. Tre screenshot commentati
Profiler o OVR Metrics: uno prima, due dopo, con una frase ciascuno su cosa mostra.

## 5. Cosa non è stato fatto e perché
Le scelte consapevoli (es. "niente occlusion culling: la scena è un bosco aperto").

## 6. Cosa ho imparato
Cinque righe. Cosa rifaresti diversamente dall'inizio.

Allegati: CSV del benchmark (prima e dopo), `CREDITS.md` degli asset.
