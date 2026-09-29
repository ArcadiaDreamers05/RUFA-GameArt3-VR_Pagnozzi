# Lezione 12 — Audio spaziale, haptics e comfort

## Audio che sta nello spazio
Un `AudioSource` con **Spatial Blend = 1** viene localizzato: più forte da vicino, dalla direzione giusta. Rolloff **logaritmico**, Min Distance (fin dove è a volume pieno) e Max Distance (oltre cui il volume non cala più): per un oggetto piccolo 0,5 / 6 m, per una stanza 1 / 15 m. Con il rolloff logaritmico il suono non arriva mai a zero: se deve sparire del tutto oltre una distanza, usa Linear o una curva Custom che scende a zero. Clip **mono** per le sorgenti 3D; Doppler a 0. **RUFA ▸ Strumenti ▸ Audio: rendi spaziali (selezione)** applica in un colpo Spatial Blend 1, rolloff logaritmico, 1 / 15 m e Doppler 0; per un oggetto piccolo abbassa poi Min e Max a mano. L'ambience (vento, notte) resta 2D ma bassa. Import della clip: Force To Mono per le sorgenti 3D, Load In Background, Compression Format Vorbis, Quality 50.

Suoni con licenza CC0: freesound.org (filtro di licenza "Creative Commons 0") e i pacchetti audio di kenney.nl. Su PC l'audio spaziale e il riverbero si sentono già (l'AudioListener è sulla camera); vibrazione e vignetta funzionano solo sul visore.

## Riverbero
Una `AudioReverbZone` per ambiente (preset Stone Room per interni in pietra, Room per stanze arredate): A mano: un oggetto vuoto al centro della stanza, `Add Component → Audio Reverb Zone`, Min Distance e Max Distance grandi come la stanza, Reverb Preset. **RUFA ▸ Strumenti ▸ Reverb zone (selezione)** ne mette una sui bounds dell'oggetto selezionato (il pavimento della stanza), con il preset Stone Room.

## Haptics
Una vibrazione breve quando afferri o premi qualcosa conferma l'azione. Il componente **Comfort Haptics** (RUFA ▸ Setup ▸ 6) aggiunge il feedback agli interactor dei controller: ampiezza 0,5 e durata 0,1 s sono un buon default; più forte è fastidioso.

## Comfort e motion sickness
Le cause: accelerazioni e rotazioni continue viste ma non sentite, frame rate instabile, orizzonte che si muove, oggetti incollati alla testa. I rimedi:
- snap turn e teleport come default; movimento continuo lento (≤ 2 m/s) senza accelerazioni;
- **vignetta** durante movimento e rotazione (**Comfort Vignette**, RUFA ▸ Setup ▸ 6);
- 90 Hz stabili (le lezioni 6–11);
- niente UI incollata alla camera: pannelli nel mondo a 0,5–1 m, testo alto almeno 1,5° di campo visivo (circa 2 cm a 1 m), menu sul polso o davanti alle mani;
- altezza del giocatore corretta (tracking a livello del pavimento), scala reale degli oggetti (lezione 4);
- sessioni brevi nel playtest, pausa se qualcuno sta male.

## Consegna
Sound design integrato (ambience, sorgenti spaziali, riverbero), haptics, vignetta, e la checklist di comfort compilata per il tuo progetto.
