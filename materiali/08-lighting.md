# Lezione 8 — Illuminazione baked: lightmap, light probe e reflection probe

## Perché il realtime costa
Ogni luce realtime si calcola per ogni pixel che tocca, ogni frame; ogni luce con ombre aggiunge una passata di rendering in più (la shadow map). Una luce puntiforme con ombre ne richiede sei, una per ogni direzione: 24 candele con ombre sono 144 shadow map a fotogramma. Su Quest si tiene realtime al massimo **una** luce direzionale (il sole) e si cuoce tutto il resto.

## I tre tipi di luce
- **Realtime**: costa ogni frame; solo per ciò che deve muoversi o cambiare.
- **Mixed**: luce diretta realtime sugli oggetti dinamici, il resto cuoce nelle lightmap. Modalità **Subtractive** = la più economica. Attenzione al chiuso: un sole Mixed in Subtractive scurisce le lightmap dove cade la sua ombra in tempo reale, cioè ovunque sotto il tetto, e lascia chiazze scure o violacee. In una scena di interni cuoci anche il sole (Baked).
- **Baked**: zero costo a runtime; la luce vive nelle lightmap (oggetti statici) e nelle probe (oggetti mobili).

## Il preset mobile
A mano: **Window → Rendering → Lighting**, scheda **Scene**, **New Lighting Settings**, poi i campi uno per uno. **RUFA ▸ Strumenti ▸ Lighting: preset mobile** li imposta tutti insieme: GI baked, lightmapper GPU, 10 texel per metro, lightmap fino a 2048, non direzionali, mixed subtractive, compressione normale. Alza la risoluzione solo dove serve (e paga in tempo di bake).

## Cosa deve essere statico
Gli oggetti che ricevono lightmap devono avere il flag **Contribute GI** (lezione 6) e UV di lightmap: nei modelli importati attiva **Generate Lightmap UVs** nell'importer, altrimenti il bake avvisa e l'oggetto resta nero o macchiato.

## Probe
- **Light Probe Group**: la luce campionata in punti dello spazio per gli oggetti mobili (chiave, oggetti afferrabili, il giocatore). A mano: **GameObject → Light → Light Probe Group**, poi **Edit Light Probes** per spostarle. **RUFA ▸ Strumenti ▸ Light Probe Group a griglia (selezione)** mette una griglia 3 × 2 × 3 dentro l'oggetto selezionato (il pavimento di una stanza, per esempio). Servono dove il giocatore va. Una probe dentro un mobile registra buio e rende nero l'oggetto che la usa: lo strumento salta le posizioni dentro gli oggetti; se le sposti a mano, tienile fuori da mobili e muri.
- **Reflection Probe**: i riflessi sui materiali lucidi. A mano: **GameObject → Light → Reflection Probe**, Type **Baked**, Resolution **128**, Box Size grande come la stanza. **RUFA ▸ Strumenti ▸ Reflection Probe (selezione)** la mette sui bounds dell'oggetto selezionato: una per stanza.

## Bake
**Generate Lighting** nella finestra Lighting, oppure **RUFA ▸ Strumenti ▸ Lighting: bake** (salva la scena e parte in background). Avvialo **presto**: mentre cuoce, sistema probe e materiali. Se non finisce in aula, finisce a casa. Dopo il bake guarda la scena dal visore: la luce cotta rimbalza sui muri chiari, quindi una luce tarata in tempo reale spesso va abbassata (anche della metà) per non bruciare le stanze piccole. **Cancella** rimuove i dati per confrontare prima/dopo.

## Consegna
Bake completo della tua scena: nessuna luce realtime tranne l'eventuale sole (mixed, e in interni meglio baked anche lui), probe dove si muove il giocatore, reflection probe per ambiente; screenshot della Lighting window e benchmark prima/dopo (GPU ms).
