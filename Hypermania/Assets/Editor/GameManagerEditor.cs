using Game;
using Steamworks;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameManager))]
public sealed class GameManagerEditor : Editor
{
    private ulong _roomId;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var gm = (GameManager)target;
        if (gm == null) return;

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Matchmaking Controls", EditorStyles.boldLabel);

        bool inPlayMode = Application.isPlaying;

        using (new EditorGUI.DisabledScope(!inPlayMode))
        using (new EditorGUILayout.VerticalScope("box"))
        {
            if (GUILayout.Button("Create Lobby"))
            {
                gm.CreateLobby();
            }

            if (GUILayout.Button("Join Lobby"))
            {
                gm.JoinLobby(new CSteamID(_roomId));
            }

            _roomId = (ulong)EditorGUILayout.LongField("Room Id", (long)_roomId);

            if (GUILayout.Button("Leave Lobby"))
            {
                gm.LeaveLobby();
            }

            if (GUILayout.Button("Start Game"))
            {
                gm.StartGame();
            }
        }
    }

}
