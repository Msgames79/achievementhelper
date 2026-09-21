# Achievement Helper For Human Fall Flat 実績管理Mod

## 機能一覧
### GUIから実績管理
`Home`キーで表示非表示を切り替え。
Unlock all achievements: 実行時に解除されていない全ての実績を解除
Unlock NSB achievements: No Stat-Based(プログレスバーがない実績)に割り当てられた実績を全て解除
Unlock SB achievements: Stat-Based(プログレスバーがある実績)に割り当てられた実績を全て解除
Unlock this achievement: 上のテキストボックスに実績のインデックスを入力すると表示される内部名の実績を解除する
Reset achievements: 実績を全てロック
Show stats: `StatsAndAchievements`内のstatに関するフィールドの値を画面に表示

### コマンド追加
`sr`: `steamreset`のエイリアス。実績を全てロック
`steamunlock <param>`または`su <param>`: 実績を解除するコマンド
`<param>`について
- 引数なし または `all`: 全て解除
- `nsb`: NSBのみ解除
- `sn`: SBのみ解除
- `<index>`: `index`番目の実績を解除
`showstats`または`ss`: `StatsAndAchievements`内のstatに関するフィールドの値を画面に表示

## インストール
1. Steam版Human Fall Flatに`BepInEx`をインストール
2. [Releases](https://github.com/Msgames79/achievementhelper/Releases/latest)からdllファイル(`AchievementHelper-(version).dll`)をダウンロード
3. `AchievementHelper-(version).dll`を`BepInEx\plugins`に置く
