using Unity.Netcode;
using UnityEngine;
using Unity.Collections;

public class GameSceneManager : NetworkBehaviour
{
    private NetworkList<FixedString32Bytes> playerNames = new NetworkList<FixedString32Bytes>();

    private void Start()
    {
        if (IsServer)
        {
            PopulatePlayerNames();
            SpawnAllPlayers();
        }
    }

    private void PopulatePlayerNames()
    {
        // Only the host should populate the list
        playerNames.Clear();

        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            var lobbyPlayer = client.PlayerObject.GetComponent<LobbyPlayer>();
            if (lobbyPlayer != null)
            {
                playerNames.Add(lobbyPlayer.playerName.Value);
            }
        }
    }

    private void SpawnAllPlayers()
    {
        int index = 0;

        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            var lobbyPlayer = client.PlayerObject.GetComponent<LobbyPlayer>();
            if (lobbyPlayer != null)
            {
                // Each player gets the correct name from the NetworkList
                string playerName = playerNames[index].ToString();
                SpawnPlayerCharacter(
                    client.ClientId,
                    lobbyPlayer.selectedCharacterId.Value,
                    playerName
                );
                index++;
            }
        }
    }

    private void SpawnPlayerCharacter(ulong clientId, int characterId, string playerName)
    {
        GameObject prefabToSpawn = GameCharacterManager.Instance.GetCharacterPrefab(characterId);
        GameObject playerCharacter = Instantiate(prefabToSpawn);

        // Assign name UI, etc.
        PlayerCharacter playerComponent = playerCharacter.GetComponent<PlayerCharacter>();
        if (playerComponent != null)
        {
            playerComponent.SetPlayerName(playerName);
        }

        playerCharacter.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
    }
}
