# Lezione 4 — Scala, level design e blockout

## La scala in VR non perdona
In Unity **1 unità = 1 metro** e nel visore lo vedi davvero: una porta di 2,5 m sembra un portone, un soffitto a 2,2 m schiaccia. Misure di riferimento (le stesse del mondo reale):

| Elemento | Misura |
|---|---|
| Altezza occhi (persona di 1,75 m) | 1,60–1,65 m |
| Porta | 80–90 × 210 cm |
| Soffitto abitazione | 2,7–3,0 m |
| Corridoio | almeno 1,2 m |
| Tavolo / scrivania | 72–76 cm; piano cucina 90 cm |
| Sedia (seduta) | 45 cm |
| Gradino | alzata 16–18 cm, pedata 28–30 cm |
| Maniglia, interruttore | 1,0–1,1 m |
| Oggetto da afferrare | tra 0,7 e 1,3 m da terra, a portata di braccio (≤ 0,7 m dal bordo dove ci si ferma) |

La scena di partenza ha un manichino di 1,75 m e una porta di 80 × 210 cm: tienili vicino a ciò che costruisci e confrontali nel visore. Altezza della testa e pavimento li decide il visore (tracking a livello del pavimento, calibrato dal Guardian): se ti senti troppo alto o troppo basso, rifai la calibrazione del pavimento nel Quest, non spostare la camera.

Modelli da Blender: unità in metri, `Apply Scale` prima di esportare; in Unity, nell'import, `Scale Factor` 1 e `Convert Units` acceso. Se il modello arriva 100 volte troppo grande o piccolo, è questo.

## Blockout: solo volumi
Il blockout è la pianta in 3D con i primitivi (**GameObject → 3D Object → Cube**), senza materiali, texture o arredo: serve a provare misure, percorsi e tempi. Regole pratiche:
- Muri come cubi da 20 cm di spessore; i primitivi hanno già il collider, quindi non li attraversi. Nomina gli oggetti per stanza (`Ingresso_MuroNord`).
- Attiva lo **snap alla griglia** nella Scene view (icona della griglia in alto, oppure tieni premuto Ctrl mentre trascini) e lavora a passi di 10 cm.
- Ogni pavimento nuovo: `Add Component → Teleportation Area`, `Interaction Layer Mask` = **Teleport** (lezione 2).
- Metti l'oggetto `Rig Bootstrap` dove il giocatore inizia, rivolto verso la prima cosa da vedere: il rig nasce lì.
- Se preferisci modellare i volumi dentro Unity, **Window → Package Manager → Unity Registry → ProBuilder** è un package ufficiale, facoltativo.
- Testa **nel visore**, non solo in editor: con **RUFA ▸ Play mode ▸ VR con Quest Link** o con una build. Il PC serve a controllare la pianta in fretta, il visore a sentire la scala.

## Guidare lo sguardo e il passo
- **Luce e contrasto**: l'occhio va dove c'è luce; una finestra o una lampada in fondo al corridoio tira avanti. Nel blockout basta una luce puntiforme provvisoria.
- **Linee e cornici**: porte, corridoi e file di oggetti puntano verso la meta. Non mettere il punto di interesse fuori asse rispetto all'ingresso di una stanza.
- **Landmark**: un oggetto grande e riconoscibile per stanza, così il giocatore sa dove si trova senza mappa.
- **Compressione e apertura**: un passaggio stretto e poi una stanza grande dà ritmo; una sequenza di stanze uguali annoia.
- **Distanze**: in VR 10 m di corridoio vuoto sono lunghi. Nel walking simulator tieni un motivo di interesse ogni 5–8 m; nell'escape room 2–3 stanze bastano per un'esperienza di 10–15 minuti.

## Concept e pitch (3 minuti a testa)
Mezza pagina, `Consegne/L04/concept.md`:
1. **Titolo e genere** (walking simulator o escape room).
2. **Ambientazione e tono** in due righe; cosa prova il giocatore.
3. **Struttura**: walking simulator → i 3–5 momenti narrativi in ordine (cosa si vede, cosa si sente, quale trigger li avvia); escape room → il flusso degli enigmi come catena "trovo X → uso X su Y → si apre Z", 3–5 passaggi, ognuno costruito con i mattoncini della lezione 3.
4. **Pianta** del livello in scala (schizzo o immagine), con il percorso del giocatore e i punti di interesse numerati.
5. **Perimetro**: numero di stanze, elenco degli asset che ti servono e da dove li prendi (solo CC0: Poly Haven, ambientCG, Kenney, The Base Mesh per le forme base del blockout), cosa tagli se manca il tempo.
Il docente approva o chiede una modifica: senza approvazione non si va avanti, e il concept approvato è quello che viene valutato all'esame.

## Consegna
In `Consegne/L04/`: `concept.md`, l'immagine della pianta, due screenshot del blockout **dal visore** (uno dall'ingresso, uno dal punto più importante). Il blockout deve essere percorribile dall'inizio alla fine con teleport o movimento continuo, con i riferimenti di scala ancora in scena.
