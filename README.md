# ダイソンスフィアプログラム日本語化プラグイン　
ダイソンスフィアプログラムを日本語化し、フォントも変更します。<br>
This plugin change all strings to Japanese and change fonts.<br>

## 機能
- 文字列を日本語に翻訳し、フォントを見やすいものに変更します。<br>
- 翻訳に伴い不具合の出たテキストの位置やボタンのサイズ等を調整します。<br>
- 翻訳用の辞書は起動時に自動でダウンロードされます。<br>
- 辞書に単語が追加されている場合は、他のMODのアイテムや技術の名前、説明文も翻訳されます。<br>


## インストールの方法（MODマネージャ「r2modman」を使うと簡単です）　
1. BepInExをインストールしてください。<br>
2. `steamapps/common/Dyson Sphere Program/BepInEx/plugins/`内に`DSPJapanesePlugin`フォルダを作成してください。<br>
3. ダウンロード後解凍してできたファイルを2で作った`DSPJapanesePlugin`フォルダに配置してください。<br>
　　　必要なファイル<br>
　　　DSPJapanesePlugin.dll<br>

## 日本語の設定方法
1.ゲームを起動すると、完全に日本語化されます。<br>
2.設定で言語を英語にしてください。

## フォントについて
本プラグインに含まれるフォントは「M+フォント」の幅を１５％狭くした上にプロポーショナル化し、英数字を「Saira」に置き換えたものです。<br>
「Saira」(https://fonts.google.com/specimen/Saira) Licensed under SIL Open Font License 1.1 (http://scripts.sil.org/OFL)<br>
「M+フォント」（http://mplus-fonts.osdn.jp/）Licensed under M+ FONTS License<br>
これ以外のフォントは指定できません。<br>

## 問い合わせ先
不具合、改善案などありましたら、DISCORD 「DysonSphereProgram_Jp」サーバー 「日本語化」チャンネル **Appun#8284**までお願いします。<br>
翻訳作業は<br>
https://docs.google.com/spreadsheets/d/1U9Y3iV7pfYGvlsl_tjvxX5mN0L_YrLlxdnCNnpMAyso/<br>
で行っております。ご協力をお願いします。<br>

## 更新履歴
### v1.2.10
- Version 0.10.30.23292に対応しました。
- 物流ステーションのUIで、文字列が全て表示されない問題を解消しました。
- 物流ステーションコントロールパネルのUIで、文字列が重なる問題を解消しました。
### v1.2.9
- Version 0.10.30.22292に対応しました。
- 星間物流ステーションのUIで、文字列が全て表示されない問題を解消しました。
### v1.2.8
- Version 0.10.28.21308に対応しています。
- ロードする度にSailIndicatorの表示が崩れていく問題を解消しました。
- 統計パネルの性能試験タブのラベルと数値がずれている問題を解消しました。
- 性能強化ツリーの現在の性能ウインドウのベルと数値がずれている問題を解消しました。
### v1.2.7
- Version 0.10.29.21950に対応しました。
- タレットウインドウの「スーパーノヴァ」の文字がはみ出る問題に対応しました。　→　訳語に改行を入れ、文字サイズ、改行幅を調整
- ブループリントウインドウでブックを選択したときの「ブループリントブックを削除する」文字列が収まらない問題に対応しました。→　ボタンの幅を拡大
### v1.2.6
- Version 0.10.29.21869に対応しました。
### v1.2.5
- Version 0.10.28.21308に対応しています。
- ロード画面の「サンドボックスツールを有効にする」の場所の調整方法の変更し、どの解像度でも問題が無いようにしました。
- マイルストーンの説明文ポップアップで、惑星名のスペースで改行されてしまう問題の解消しました。
### v1.2.4
- Version 0.10.28.21196に対応しています。
- パフォーマンスモニターのフォントを変更しました。
- 実績パネルの表示を調整しました。
- ダークフォグモニターの基地情報の表示を調整しました。
- ピックアップフィルター画面の表示を調整しました。
- 死亡画面のメニューの表示を調整しました。。
- アセンブラーウインドウ等の増産剤のメニューを調整しました。
- Zメニューの表示を調整しました。
- メカパネルの表示を調整しました。
- サンドボックスツールのタイトルを拡大しました。
- 一時的にオミットしていたUIの調整の残りの一部を復活しました。
### v1.2.3
- 一時的にオミットしていたUIの調整の一部を復活しました。
### v1.2.2
- Version 0.10.28.21014に対応しました。
- 戦場分析基地ウインドウのUIを調整しました。
- タレットウインドウのUIを調整しました。
- UIの調整は一部対応できていません。
### v1.2.1
- Version 0.10.28.20856にとりあえず対応しました。
- UIの調整は一部対応できていません。
### v1.1.10
- 他のMODとの干渉によるエラーを解消しました。
### v1.1.9
- Version 0.9.26.12900に対応しました。
- 恒星/惑星詳細情報パネルの数字フォントが変更になったことに対応しました。
- ロード画面の「サンドボックスツールを有効にする」の位置を調整しました。
- 合成機ウインドウの無料アイテム関係の表示を調整しました。
- セーブ画面のクラウド保存の警告の位置を調整しました。
- 気まぐれにMODのアイコンを変更してみました。
### v1.1.8
- 辞書自動アップデートをオフにした場合エラーが出ていた問題を解消しました。
### v1.1.7
- Version 0.9.25.11996に対応しました。（ロード画面のメタデータ寄与量の表示の問題を解消）
### v1.1.6
- 新規ゲーム画面の星のスペクトル型名が間違っていた問題を解決しました。
- MOD「BlueprintTweaks」と同時に使う場合に生じるエラーを解決しました。
- メカカスタム画面のUIの表示を修正しました。
- 初回起動時の言語選択画面で「日本語」を選択できるようにしました。
### v1.1.5
- γ線レシーバーのUIの問題に対応しました。
### v1.1.4
- ブループリントの読み書きでエラー出ていた問題を解決しました。
- GitHubに公開しました。
### v1.1.3
- Version 0.9.24.11209に対応しました。
- 高度採掘機のUI、メカカスタム画面のUI、ダイソンスフィア画面のUI、アイテムの増産剤追加効果説明UIの表示の問題を解決しました。
- 辞書のダウンロード方法を変更しました。
- 翻訳作業所のシートからデータを取り込んで辞書ファイルを作る機能が復活しました。
- 翻訳者向けにバージョンアップ時の新規文字列を書き出せるようになりました。
### v1.1.2
- 不要ファイルを削除し忘れていたため、アップしなおしました。
### v1.1.1
- 時間の経過に伴いメモリ使用量が増えていた問題を解決しました。
- 辞書の自動アップデート機能をMOD本体のDLLに組み込みました。それに伴いファイル構成が変更されています。
- 以前のバージョンの、DSPJPTranslationUpdater.dll、updater_settings.txt、Translationフォルダは削除してください。
- 現在、翻訳作業所のシートからデータを取り込んで辞書ファイルを作る機能は削除されています。
### v1.1.0
- トラフィックモニタウインドウのUIの文字列が収まらない問題に対応しました。無駄な処理を減らしました。
### v1.0.9
- ブループリントウインドウのUIの文字列が収まらない問題に対応しました。
### v1.0.8
- 銀河系ビューの読み込みエラーメッセージが一部だけしか表示されない問題に対応しました。ランダムチップがどんどん大きくなるバグに対応しました。
### v1.0.7
- アップデート0.7.18.6931にとりあえず対応しました。<br>
### v1.0.6
- 言語設定で日本語以外を選んでも、MODで追加したアイテムが日本語化されていた問題を解決しましました。
### v1.0.5
- 航行モードのインジケーターウインドウをとりあえず日本語化しました。いくつかのMODの翻訳に対応しました。翻訳方法をちょっと変えました。
### v1.0.4
- フォントデータをDLLに埋め込みました。強制日本語化から言語選択で日本語を選ぶ方法に切り替えました。星図で星の名前が輝きすぎている問題を解決しました。
### v1.0.3
- r2modman用に依存関係を記載しました。MOD本体の機能の変化はありません。
### v1.0.2- 
アイテムを追加するMODによってはエラー出ていた問題を解決しました。
### v1.0.1
- r2modmanで導入した際の不具合を解消しました。（フォルダ名が変更されることへの対応）
### v1.0.0
- アップグレードモード「□コンベアベルト」の位置を調整しました。Thunderstoreでリリースしました。




