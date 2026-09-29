# Lezione 6 — Geometria: poly count, LOD, culling

## Quanto può disegnare un Quest 3S
Regola pratica: tra 500.000 e 1.000.000 di triangoli **a schermo** a 90 Hz, con margine per tutto il resto. Meno è meglio: ogni triangolo in più costa vertex shading e, se piccolo, spreca la GPU. Conta sempre "a schermo", non "nella scena": frustum culling (gratis), occlusion culling e LOD servono proprio a ridurre la differenza.

## Come contare
- **Stats** in Game view (Tris, Verts): cosa viene disegnato in quel momento. Attenzione: conta anche le passate delle ombre, quindi con molte luci in tempo reale con ombre lo stesso oggetto viene contato più volte e il numero può essere molto più alto dei triangoli della scena.
- **Profiler → Rendering**: triangoli, vertici, draw call per frame, anche sul visore.
- **RUFA ▸ Strumenti ▸ Conta triangoli (selezione)**: quanti triangoli pesano gli asset selezionati (tutti i LOD compresi).

## LOD Group
Un `LODGroup` mostra mesh via via più semplici quando l'oggetto occupa meno schermo. Tre livelli bastano: 100 %, ~30 %, ~10 % dei triangoli, soglie 50 % / 15 % / 3 % di altezza a schermo.
- **In Blender**: modificatore Decimate (Ratio 0,3 e 0,1), esporta `nome_LOD1.fbx` e `nome_LOD2.fbx` **nella stessa cartella** del modello originale.
- **In Unity, a mano**: trascina `nome_LOD1` e `nome_LOD2` dentro l'oggetto, come figli nella stessa posizione; sull'oggetto `Add Component → LOD Group`; clic sulla fascia **LOD 1** e trascina lì il figlio `nome_LOD1`, poi lo stesso per **LOD 2** (la fascia LOD 0 tiene la mesh originale).
- **RUFA ▸ Strumenti ▸ LOD Group dalle varianti selezionate** fa tutto questo su ogni oggetto selezionato. Trascina la barra dei LOD nell'Inspector per vedere i cambi.

## Static flags
Seleziona tutto ciò che non si muove (muri, pavimenti, arredo fisso). A mano: nell'Inspector, la freccia accanto a **Static** in alto a destra, e spunta Occluder Static, Occludee Static, Batching Static, Contribute GI e Reflection Probe Static. **RUFA ▸ Strumenti ▸ Statici: imposta flag** mette gli stessi flag su tutta la selezione e ai vetri lascia spento Occluder, perché un vetro non nasconde ciò che ha dietro. Occluder/Occludee servono all'occlusion culling, Batching al combining delle mesh (lezione 7), Contribute GI e Reflection Probe al lighting (lezione 8).

## Occlusion culling
Con muri e stanze, ciò che sta dietro un muro non va disegnato. Il bake si fa da **Window → Rendering → Occlusion Culling**, scheda **Bake**: Smallest Occluder 0,5, Smallest Hole 0,25, Backface Threshold 100, poi **Bake**. **RUFA ▸ Strumenti ▸ Occlusion culling: bake** fa lo stesso con gli stessi valori; in Scene view, Occlusion Culling → Visualize mostra cosa viene tagliato. Non serve in spazi aperti senza occluder grandi. **Cancella** rimuove i dati per confrontare.

## Consegna
Nel tuo sandbox: almeno due asset con LOD Group, static flags su tutto ciò che è fisso, occlusion culling baked se hai stanze; benchmark prima/dopo (stesse stazioni, build non development) con i triangoli a schermo e il frame time nella tabella del report.
