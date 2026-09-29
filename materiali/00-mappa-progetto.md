# Come funziona il sandbox

Cosa c'è nel progetto, cosa succede quando premi Play, dove si cambiano le cose e dove guardare quando qualcosa non va. Leggila con Unity aperto, e alla fine fai le prove del §7.

## 1. Cosa c'è nel repository

| Cartella | Cos'è |
|---|---|
| `sandbox/` | Il progetto Unity: lo apri da Unity Hub e diventa il tuo progetto d'esame |
| `sandbox/Assets/Scenes/Sandbox.unity` | La scena di partenza |
| `sandbox/Assets/Demo/` | La casa del docente, con la scena `Assets/Demo/Scenes/Demo.unity`: la curi insieme a lui durante le dimostrazioni. A ogni `git aggiorna` torna uguale alla sua |
| `sandbox/Assets/RUFA/Prefabs/` | Gli otto prefab mattoncino: chiave, serratura, porta, pulsante, trigger narrativo, pannello UI, audio spaziale, oggetto afferrabile. Li usa anche la scena del demo: per le tue versioni fai una copia, o una Prefab Variant, in `Assets/Progetto` |
| `sandbox/Assets/Samples/XR Interaction Toolkit/3.6.1/Starter Assets/` | Il rig VR di XR Interaction Toolkit, con la locomozione |
| `sandbox/Packages/com.rufa.core/` | Il nucleo del corso: il doppio target VR/PC, gli script dei mattoncini e il menu RUFA. **Non va modificato**: arrivano da qui gli aggiornamenti del docente |
| `materiali/` | Le dispense |
| `Consegne/` | Le tue consegne, una cartella per lezione (la crei tu) |

## 2. La scena salvata e quello che nasce in Play

In edit mode la Hierarchy mostra solo ciò che è salvato nella scena. Premendo Play compaiono altri oggetti, creati dagli script all'avvio, che allo Stop spariscono.

| Salvato nella scena | Nasce in Play |
|---|---|
| `Directional Light`, `Floor` (20 × 20 m, con `Teleportation Area`), i due riferimenti di scala, `Rig Bootstrap`, `Mini-esempi` | `XR Interaction Manager` e il giocatore: `Desktop Rig` su PC, `XR Origin (XR Rig)(Clone)` in VR |
| Dalla lezione 5 `Benchmark` con le stazioni (RUFA ▸ Setup ▸ 5); dalla lezione 12 `Comfort Vignette` e `Comfort Haptics` (RUFA ▸ Setup ▸ 6) | Dalla lezione 12, in VR, la vignetta dentro la camera |

Il giocatore **non è nella scena**: non cercarlo lì e non trascinarcelo.

**La scena del demo** (`Assets/Demo/Scenes/Demo.unity`) funziona allo stesso modo, con più radici. Le principali:

- `Casa`, cioè muri, pavimenti e soffitti;
- `Luci`, il sole e le candele;
- `Arredo`;
- `Geometria`, che in edit mode è vuota e si riempie in Play con pavimenti e colonne fittissimi;
- `Enigma`, `Benchmark` e `Demo Pipeline`;
- `Teacher Menu`, il pannello del docente, che nella tua copia resta spento.

`Demo Pipeline` in Play passa al livello di qualità **Demo**, che usa l'asset URP del demo, pieno di difetti. Allo Stop si torna al livello di prima. In edit mode la Scene view del demo usa l'asset del progetto: **i numeri del demo si leggono sempre in Play**.

## 3. Cosa succede quando premi Play

Unity chiama gli script in tre fasi: prima `Awake` su tutti gli oggetti, poi `Start` su tutti, poi `Update` a ogni fotogramma.

1. **Awake.** `Rig Bootstrap` crea `XR Interaction Manager`, il centralino che mette in contatto chi afferra (interactor) e cosa si afferra (interactable). Poi controlla se un visore sta pilotando l'app:
   - **no** (Play normale, build Windows): costruisce `Desktop Rig`, con `Main Camera` all'altezza degli occhi, un raggio che parte dal centro dello schermo, la `Mano` dove arriva ciò che afferri e il mirino;
   - **sì** (build Quest, Play con **RUFA ▸ Play mode ▸ VR con Quest Link**): crea il rig VR dal prefab `XR Origin (XR Rig)` e sul Quest chiede i 90 Hz.
2. **Start.** Il giocatore esiste, e da qui in poi gli altri oggetti lo possono trovare: per esempio le `Teleportation Area` trovano da sole il teleport del rig quando serve.
3. **A ogni fotogramma:** tastiera, mouse e controller muovono il giocatore; su PC il tasto B, se in scena c'è `Benchmark`, avvia la misura.

## 4. Come si parlano gli oggetti

- **Interactor e interactable.** Il raggio o la mano (interactor) afferra o preme gli oggetti (interactable: chiave, porta, pulsante). La serratura è un socket, cioè un interactor fermo: "afferra" la chiave quando entra nella sua zona, ma solo se la chiave ha il layer di interazione **Chiave**.
- **Eventi nell'Inspector.** La serratura non sa niente della porta: il suo evento `On Unlocked` chiama l'apertura della porta. Il collegamento si vede e si cambia nell'Inspector, senza scrivere codice. Stesso schema per `On Pressed` del pulsante e per l'evento del trigger narrativo.
- **Il trigger narrativo** riconosce il giocatore dal CharacterController, che hanno entrambi i rig.
- **Su PC** il raggio del `Desktop Rig` funziona come quelli dei controller: per questo gli stessi oggetti si usano con il mouse e con il visore, senza codice in più.

## 5. Dove si cambiano le cose

| Vuoi cambiare | Dove |
|---|---|
| Velocità, rotazione, teleport (lezione 2) | Nel **prefab** `XR Origin (XR Rig)` in `Assets/Samples/…/Starter Assets/Prefabs/`, non sulla copia `(Clone)` che compare in Play. Il demo usa lo stesso rig: le modifiche valgono anche lì |
| Come si impugna un oggetto | Il figlio `Attach` dell'oggetto afferrabile |
| Cosa succede quando la chiave entra o premi il pulsante | Gli eventi `On Unlocked` e `On Pressed` nell'Inspector |
| Le cure della dimostrazione | Nella scena del demo, sugli stessi oggetti del docente: lui dice finestra, oggetto, componente, campo e valore |
| Il demo sul visore | **RUFA ▸ Build ▸ Demo (APK)**: un'app a parte, "RUFA Demo", accanto a "RUFA Sandbox" |
| Il tuo livello | La scena `Sandbox.unity`, oppure una scena nuova: in quel caso copiaci `Rig Bootstrap` e aggiungila alla **Scene List** di **File ▸ Build Profiles**, perché le build RUFA prendono le scene da lì |
| Impostazioni di progetto, URP, XR | Solo quando una lezione lo chiede (lezioni 8–10) |
| Il nucleo `com.rufa.core` | Mai |

## 6. Dove guardare quando qualcosa non va

La prima cosa da guardare è sempre la **Console**: ogni voce del menu RUFA scrive una riga `RUFA: ...` con l'esito o con il motivo per cui non ha fatto niente.

| Sintomo | Dove guardare |
|---|---|
| In Play non c'è il giocatore ("No cameras rendering") | L'oggetto `Rig Bootstrap` manca o è disattivato; nel suo Inspector, il campo `Xr Rig Prefab` |
| Con il visore collegato il Play resta da PC | **RUFA ▸ Play mode ▸ VR con Quest Link** senza spunta, oppure Quest Link non attivo |
| La chiave non entra nella serratura | L'`Interaction Layer Mask` della chiave e quello della serratura: tutti e due devono avere **Chiave** |
| Il teleport non va su un pavimento | Manca `Teleportation Area`, oppure il suo `Interaction Layer Mask` non è **Teleport** |
| Le modifiche spariscono allo Stop | Le hai fatte in Play, o sulla copia `(Clone)` invece che sul prefab |
| Il trigger non scatta e ci sbatti contro | Nel suo collider manca la spunta **Is Trigger** |
| Nel demo i numeri in edit mode non tornano con quelli del docente | In edit mode la Scene view usa l'asset URP del progetto: misura in Play |
| Il demo è diverso da quello del docente | Unity chiuso, poi `git aggiorna` |

## 7. Prove da fare per capirlo

Ogni prova ha un risultato atteso: se lo vedi, quel pezzo l'hai capito. Alla fine Ctrl+Z, o esci senza salvare.

1. In edit mode guarda le radici nella Hierarchy. Play: compaiono `XR Interaction Manager` e `Desktop Rig`. Stop: spariscono.
2. In Play apri `Desktop Rig`: dentro ci sono `Main Camera`, la `Mano` e il mirino.
3. Disattiva `Rig Bootstrap` (la spunta accanto al nome, nell'Inspector) e premi Play: niente giocatore, "No cameras rendering". Riattivalo.
4. In `Mini-esempi` seleziona la serratura e trova `On Unlocked`. Togli il collegamento e riprova: la chiave entra, la porta resta chiusa.
5. Sulla chiave lascia nell'`Interaction Layer Mask` solo Default: la chiave non entra più.
6. Sul pavimento disattiva `Teleportation Area`: in VR l'arco del teleport non trova più un punto valido e non ti fa saltare.
