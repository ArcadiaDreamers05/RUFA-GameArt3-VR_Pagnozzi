# Lezione 3 — Interazioni: trigger, grab, socket, porte, UI nel mondo

## I prefab mattoncino
Gli otto prefab sono in `Assets/RUFA/Prefabs/` e i layer di interazione hanno già un nome (**Chiave** = 1, **Teleport** = 31: Project Settings → XR Plug-in Management → XR Interaction Toolkit → Interaction Layers). Nella scena, sotto `Mini-esempi`, ci sono i due esempi montati: guardali prima di costruire i tuoi. Se li cancelli, **RUFA ▸ Setup ▸ 3** e **4** li rimettono (il 3 riscrive i prefab da zero).

| Prefab | Cosa fa | Come si usa |
|---|---|---|
| `Afferrabile` | Cubo di 10 cm con `Rigidbody` e `XR Grab Interactable`; il figlio `Attach` decide come si impugna | Sostituisci la mesh del cubo con la tua, sposta `Attach` dove vuoi la mano |
| `Chiave` | Una chiave antica di 15 cm con `Rigidbody` e `XR Grab Interactable`, sul layer **Chiave** oltre a Default; si impugna per l'anello (figlio `Attach`) | È l'unico oggetto che la `Serratura` accetta. Per farne accettare un altro, aggiungi **Chiave** al suo `Interaction Layer Mask` |
| `Serratura` | La bocchetta di una serratura, con `XR Socket Interactor` (accetta solo il layer Chiave, zona di cattura di 20 cm davanti alla bocchetta) e componente `Lock`. La chiave entra dritta, con l'anello fuori | Mettila sulla faccia di una porta, come figlia di `Anta` sotto il pomello: così si muove con la porta. Collega `On Unlocked` a quello che deve succedere (una porta, una luce, un suono). Scatta una volta sola |
| `Porta` | Radice sul cardine → `Anta` (ruota) → `Pannello` (porta a pannelli da 90 × 210 cm) e due pomelli; componente `Door` | Posiziona la **radice** dove sta il cardine. `Open Angle` 90°, `Duration` 0,6 s. Con `Open On Select` si apre anche toccando o cliccando l'anta; spegnilo se deve aprirsi solo con la chiave |
| `Pulsante` | Base con `XR Simple Interactable` e cappello che si abbassa; componente `Push Button` | Collega `On Pressed` a ciò che vuoi. Si preme afferrando o cliccando |
| `TriggerNarrativo` | Volume trigger 2 × 2 × 2 m; componente `Narrative Trigger` | Scrivi `Text`, trascina un `PannelloUI` in `Panel` (il figlio `Testo`) e un `AudioSpaziale` in `Audio Source`. `Show Seconds` = durata del testo, `Once` = scatta una sola volta, `On Entered` = altri effetti. Riconosce il giocatore dal `CharacterController`, quindi funziona con il visore e con il PC |
| `PannelloUI` | Canvas world space 60 × 40 cm (scala 0,001), sfondo scuro, testo TextMeshPro | Testo da leggere, non cliccabile. Mettilo a 0,5–1 m dal punto in cui il giocatore lo guarda, altezza occhi (1,5–1,7 m) |
| `AudioSpaziale` | `AudioSource` 3D (Spatial Blend 1, rolloff logaritmico, 1–15 m), loop, parte da solo, **senza clip** | Trascina una clip **mono** con licenza CC0 in `AudioClip`. Se deve partire da un trigger, spegni `Play On Awake` |

I modelli di chiave, serratura e porta sono CC0 (The Base Mesh). Puoi usarne di tuoi, con gli stessi componenti: lavora su una copia dei prefab, o su una Prefab Variant, in `Assets/Progetto`, perché gli originali li usa anche la scena del demo.

## Collegare un evento nell'Inspector
Sull'evento (`On Unlocked`, `On Pressed`, `On Entered`) premi **+**, trascina nella casella l'oggetto da comandare, poi dal menu a tendina scegli il componente e la funzione: `Door.Open`, `Door.Toggle`, `GameObject.SetActive`, `AudioSource.Play`, `Light.enabled`. È lo stesso meccanismo per tutto il corso. L'esempio `Serratura → Porta.Open` di Setup 4 è già collegato così.

## Gli stessi oggetti sul visore e sul PC
| | Quest | PC |
|---|---|---|
| Afferrare e portare | Tieni premuto il **grip** | Punta il mirino sull'oggetto (diventa **giallo**, fino a 3 m) e tieni premuto il **tasto sinistro**: l'oggetto ti arriva in mano, in basso a destra, e ci resta finché tieni premuto |
| Chiave nella serratura | Porta la chiave alla serratura e rilascia il grip | Guarda la serratura (il mirino diventa **verde**) e rilascia: la chiave entra da sola, non serve abbassarsi. Rilasciato altrove, l'oggetto cade |
| Usare (activate) | **Trigger** mentre tieni l'oggetto | **Tasto destro** |
| Porta, pulsante | Grip sull'anta o sul cappello | Click sinistro |
Il rig PC usa lo stesso tipo di interactor delle mani del visore (un ray interactor), per questo non serve codice diverso. Il mirino del PC preme solo oggetti 3D (porta, pulsante), non i pulsanti di una UI: per un comando da premere usa il prefab `Pulsante`. Le mani vedono solo il layer **Default**, il teleport solo **Teleport**: un oggetto sul layer sbagliato è invisibile.

## Quando non funziona
| Problema | Verifica |
|---|---|
| La chiave non entra | La chiave ha il layer **Chiave** e un `Rigidbody`; la serratura ha `Interaction Layer Mask` = Chiave; in VR rilasci entro 20 cm dalla serratura, su PC con il mirino verde |
| Su PC il mirino non diventa verde sulla serratura | Sei a più di 3 m, guardi la porta dal lato senza serratura, oppure tra te e la serratura c'è un collider. Oppure la serratura non accetta quello che hai in mano (layer diverso) o ha già dentro una chiave |
| L'oggetto si impugna storto | Sposta o ruota il figlio `Attach` |
| La porta non si apre con la chiave | `On Unlocked` della `Serratura` è vuoto o punta alla funzione sbagliata: deve essere `Door.Open` |
| La porta ruota intorno al centro | Hai spostato `Anta` invece della radice: la radice sta sul cardine, `Anta` resta a (0, 0, 0) |
| Il trigger non scatta | Il collider ha `Is Trigger` acceso; il volume è attraversato davvero: in VR chi si teletrasporta oltre il volume senza finirci dentro non lo attiva, quindi fallo largo quanto il passaggio |
| Il testo non compare | `Panel` punta al figlio `Testo` del pannello, non alla radice |
| Il pannello non si legge in VR | È troppo piccolo o troppo lontano: 60 × 40 cm a 1 m con testo 32 è il minimo; non ridurre la scala 0,001 |
| L'audio non si sente | Manca la clip, oppure `Max Distance` è troppo corto; la clip deve essere mono per la spazializzazione |

## Consegna
In `Consegne/L03/`: un video di 30 secondi (sul visore: pulsante Meta → Fotocamera → Registra; su PC: Win+G) in cui si vedono un trigger narrativo, un grab, la chiave nella serratura e una porta che si apre. Le clip e i modelli usati devono essere CC0.
