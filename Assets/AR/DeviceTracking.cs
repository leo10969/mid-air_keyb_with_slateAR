using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation; // AR Foundationの名前空間を使用
using System.IO; // ファイル書き込みのために必要
using System.Text; // StringBuilderの使用
using UnityEngine.UI; // UIコンポーネントの使用
using TMPro;

namespace MediaPipe.HandPose
{
    public class DeviceTracking : MonoBehaviour
    {
        private ARCameraManager arCameraManager;
        private List<Vector3> positions; // 位置データを格納するリスト
        private bool isTracking = false; // トラッキング状態のフラグ

        public TextMeshProUGUI deviceTrackingText;
        public Button trackingButton; // トラッキングボタン
        public ExportCsvScript exportCsvScript; // CSVエクスポートスクリプト

        void Start()
        {
            // ARCameraManagerを取得
            arCameraManager = FindObjectOfType<ARCameraManager>();
            positions = new List<Vector3>();

            // // ボタンのリスナー設定
            // trackingButton.onClick.AddListener(ToggleTracking);
        }

        void ToggleTracking()
        {
            // トラッキング状態の切り替え
            isTracking = !isTracking;

            if (isTracking)
            {
                // トラッキングを開始
                StartCoroutine(TrackPosition());
                deviceTrackingText.text = "Device Tracking Off";
            }
            else
            {
                deviceTrackingText.text = "Device Tracking On";
                // トラッキングを停止し、データをCSVに保存
                StopCoroutine(TrackPosition());
                SavePositionsToCSV();
                positions.Clear(); // データリストをクリア
            }
        }

        IEnumerator TrackPosition()
        {
            while (isTracking)
            {
                if (arCameraManager != null && arCameraManager.subsystem != null && arCameraManager.subsystem.running)
                {
                    // デバイスの現在の位置を取得してリストに追加
                    Vector3 currentPosition = arCameraManager.transform.position;
                    positions.Add(currentPosition);

                    // デバッグ用のログ出力
                    Debug.Log($"Tracking Position: {currentPosition}");
                }
                yield return null; // 次のフレームまで待機
            }
        }

        void SavePositionsToCSV()
        {
            // ファイル名に追加するサフィックスを初期化
            string filterSuffix = "";

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("timestamps,X,Y,Z");
            foreach (Vector3 pos in positions)
            {
                sb.AppendLine(System.DateTime.Now.ToString("yyyyMMddHHmmss") + $",{pos.x},{pos.y},{pos.z}");
            }
            string baseFileName = "/device_positions_" + exportCsvScript.lastpath;
            string filePath = UnityEngine.Application.persistentDataPath + baseFileName + filterSuffix + ".csv";
            File.WriteAllText(filePath, sb.ToString());
            Debug.Log(filePath);
        }
    }
}