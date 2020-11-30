using System.Linq;

namespace Game.Net
{
    public class ServerScene
    {
        private readonly ServerLobby _lobby;

        public ServerScene(ServerLobby lobby)
        {
            _lobby = lobby;
            _lobby.OnUserLoadedToScene += UserLoadedToScene;
        }
        
        public string Current { get; private set; }

        public void ChangeServerScene(string newScene)
        {
            Current = newScene;
            
            var readyUsers = _lobby.ReadyUsers;

            foreach (var userConnection in readyUsers)
            {
                userConnection.SceneState = UserConnection.UserSceneState.IsLoading;
            }   
            
            _lobby.NetworkManager.ServerChangeSceneWith(newScene,
                readyUsers.Select(x=>x.Connection));
        }

        /// <summary>
        /// Меняет сцену у заданного пользователя.
        /// </summary>
        /// <param name="userConnection"></param>
        public void ChangeUserScene(UserConnection userConnection)
        {
            userConnection.SceneState = UserConnection.UserSceneState.IsLoading;
            
            _lobby.NetworkManager.ServerChangeSceneFor(userConnection.Connection);
        }
        
        
        private void UserLoadedToScene(UserConnection userConnection)
        {
            userConnection.SceneState = UserConnection.UserSceneState.Loaded;
        }
    }
}