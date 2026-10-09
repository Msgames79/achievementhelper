# Achievement Helper For Human Fall Flat 実績管理Mod

## 機能一覧
### GUIから実績管理
`Home`キーで表示非表示を切り替え。

Unlock all achievements: 実行時に解除されていない全ての実績を解除

Unlock NSB achievements: No Stat-Based(プログレスバーがない実績)に割り当てられた実績を全て解除

Unlock SB achievements: Stat-Based(プログレスバーがある実績)に割り当てられた実績を全て解除

Reset achievements: 実績を全てロック

Unlock this achievement: 左にある実績のインデックスを指定して解除

Show stats: `StatsAndAchievements`内のstatに関するフィールドの値を画面に表示

### コマンド追加
基本コマンド : 
```
achievementhelper <commands> <option>
```
または
```
ah <commands> [<option>]
```
(`option`はコマンド次第でありません)
### `commands`一覧(1行目はフルネームによる記述、2行目はエイリアスを使用した最短記法)
```
achievementhelper unlock [<option>]
```
```
ah u [<option>]
```
実績を解除するコマンド
`<option>`について
- 引数なし または `all`: 全て解除
- `nsb`: NSBのみ解除
- `sb`: SBのみ解除
- `<index>`: `index`番目の実績を解除(1始まり)
```
achievementhelper reset
```
```
ah r
```
解除した実績を全てリセットするコマンド
```
achievementhelper showstatus
```
```
ah ss
```
Statsなどの表示/非表示を切り替え(**Stats**だけではないので**status**になっています)


## インストール
1. Steam版Human Fall Flatに`BepInEx`をインストール
2. [Releases](https://github.com/Msgames79/achievementhelper/Releases/latest)からdllファイル(`AchievementHelper-(version).dll`)をダウンロード
3. `AchievementHelper-(version).dll`を`BepInEx\plugins`に置く

またはPowerShellで
```ps1
Invoke-RestMethod -Uri "https://raw.githubusercontent.com/Msgames79/achievementhelper/refs/heads/main/install.ps1" | Invoke-Expression
```

## 過去の機能
### コマンド追加(v1.2.0まで)
`sr`: `steamreset`のエイリアス。実績を全てロック

`steamunlock <param>`または`su <param>`: 実績を解除するコマンド

`<param>`について
- 引数なし または `all`: 全て解除
- `nsb`: NSBのみ解除
- `sb`: SBのみ解除
- `<index>`: `index`番目の実績を解除(1始まり)

`showstats`または`ss`: `StatsAndAchievements`内のstatに関するフィールドの値を画面に表示

## バージョン履歴
### 1.0.0
初回リリース

### 1.1.0
- Stats表示UIの文字サイズと色を変更できるようにした
- Statsの増加時に増分を表示(JumpとDrown以外)
- Jumpが増えるかどうかのフラグを表示(中身はStateが`HumanState.Jump`以外かどうか判定)

### 1.1.1
- 足りない`return`を追加

### 1.2.0
- `StatsAndAchiEvements.Save()`のタイミング表示
- 直近で解除した実績の表示
- 解除した数の表示(All、SB、NSB)

### 1.3.0
- 大幅なUI刷新(入力ボックスに見える場所は入力バリデーションがめんどくさいので入力できないようにしている)
- Statusの内容を部分的に変更、順番の入れ替え、文字サイズと色のリアルタイム反映
- アンカー付きのテキスト移動も可能
- コマンド構文の変更(`ah`を中心にコマンド展開)
- Stats系の実績をStatsを直接書き換えして解除