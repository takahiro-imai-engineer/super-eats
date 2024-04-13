SuperEatsのポートフォリオ用リポジトリ

# 前提

- SuperEatsは、下記でからダウンロードすることが可能です。
  - [GooglePlay](https://play.google.com/store/apps/details?id=com.newgameplus.supereats)
  - [AppStore](https://apps.apple.com/jp/app/super-eats-%E3%82%A8%E3%82%AF%E3%82%B9%E3%83%88%E3%83%AA%E3%83%BC%E3%83%A0%E9%85%8D%E9%81%94%E5%93%A1/id6475675058)

|街ステージ|未来ステージ|世紀末ステージ|
|---|---|---|
| <img width="1400" src="https://github.com/takahiro-imai-engineer/newgameplus-1/assets/69511542/e2ace094-6003-4dc4-8866-14942af724bc"> | <img width="1400" src="https://github.com/takahiro-imai-engineer/newgameplus-1/assets/69511542/af4ec335-2967-4b21-b6a4-12ed9c540c89"> | <img width="1400" src="https://github.com/takahiro-imai-engineer/newgameplus-1/assets/69511542/3591133c-1f75-459c-a1f2-67e8f236184b"> |

- ゲームのジャンルは、ランゲーム。

> [!WARNING]
> - 有料アセットは、パブリックリポジトリにするにあたって、  
>   プロジェクトから取り除いているため、エラーとなりゲームを再生することはできません。
>   - 使用していた有料アセットについては、一部後述しています。


# 環境

- Unity 2021.3.10f1
- UnityHubで開くときは、PlatformをAndroidかiOSを選択


# 各シーンの場所

- `/client/Assets/App_Project/Scene`がシーンファイル
  - Main
    - ビルドした際に設定する初期シーン
  - Title
  - InGame
    - このシーンファイルを基本的に開きます。
  - WarpNextStage
    - エンディング用シーン

# ゲームの再生の仕方

- Unity で`/client`を選択して、開く
- `/client/Assets/App_Project/Scene`内のシーンファイルを選択

# client 下のフォルダ説明

- App_Game
  - 他のプロジェクトでも共通のスクリプト(ダイアログ、Android のプラグイン等)

- App_Project
  - プロジェクト特有のアセットはこちらに格納
  - このプロジェクトで作成したスクリプトやプレハブはこちらに格納されています。

- App_System
  - ロングプレス、シングルトン等のスクリプト等

- External Assets
  - 有料アセット等

- Resources
  - マスターデータや利用規約のテキスト等

- AssetPacks
  - GoolePlayのPlayAssetDeliveryで配信するアセットバンドルを格納するフォルダ

- StreamingAssets
  - Andorid/iOSのアセットバンドルを格納

# 有料アセット一部抜粋
- ステージの配置物やモーション、エフェクトは省略
  - 3つ(街/未来/世紀末)の世界観に合うように有料アセットを購入して使用しました。

- シェーダー系
  - [CurvedWorld](https://assetstore.unity.com/packages/vfx/shaders/curved-world-173251)
    - ステージを曲げて見せるシェーダー
  - [VolumetricLightBeam](https://assetstore.unity.com/packages/vfx/shaders/volumetric-light-beam-99888?locale=ja-JP)
    - ステージのスポットライト等
  - [Easy performant outline](https://assetstore.unity.com/packages/vfx/shaders/fullscreen-camera-effects/easy-performant-outline-2d-3d-urp-hdrp-and-built-in-renderer-157187#description)
    - アウトラインアセット

- ツール系
  - [EasySave3](https://assetstore.unity.com/packages/tools/utilities/easy-save-the-complete-save-data-serializer-system-768)
    - セーブデータのセーブ・ロード・暗号化
  - [DOTweenPro](https://assetstore.unity.com/packages/tools/visual-scripting/dotween-pro-32416)
    - Tween。ステージのギミックやUI演出に使用
  - [MeshSimplify](https://assetstore.unity.com/packages/tools/modeling/mesh-simplify-43658)
    - モデルのポリゴン数削減に使用


# 導入しているSDK
- Firebase Analytics
  - 計測/カスタムイベント
- Tenjin
  - 計測用
- IronSource
 - 広告再生
 - UnityAds/AdMobを導入済み
- GooglePlay
  - AppBundle
  - AssetDelively
