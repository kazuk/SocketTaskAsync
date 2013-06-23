# SocketTaskAsync

SocketTaskAsync は Socket の ConnectAsync/AcceptAsync 等 SocketAsyncEventArgs を利用するメソッド類をTAPLでの非同期に対応させるクラスライブラリです。

## 方針

TaskCompletionSource<T> を使って SocketAsyncEventArgs でのイベント等を Task の完了であったり、Taskの失敗にマップする事を方針としています。

SocketEventArgs を内部で使用する版と文脈オブジェクトとして受け渡す 2バージョンを混載しています。
使うには前者の方が楽ですが、SocketAsyncEventArgs を繰り返し確保しメモリにちっともやさしくない事になります。

## 提供API

SocketTaskAsync.Extentions.SocketTaskAsyncExtention から Socket に対する拡張メソッドでほとんどのAPIを提供します。




## ここまでで問題になった事の解決策

### SocketException が SocketError から作れない

AsyncSocketErrorExceptionを作ってそれを投げるようにした。

### TaskComletionSource<void> が作れない

作れないので、後続タスクが使いそうな値を返すようにします。最悪思いつかなければ SocketError 値を返します。
