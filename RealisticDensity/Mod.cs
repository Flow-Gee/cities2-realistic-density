using Colossal.Logging;
using Game;
using Game.Modding;
using RealisticDensity.Systems;
using System;
using Unity.Entities;

namespace RealisticDensity
{
    public class Mod : IMod
    {
        public const string Name = "RealisticDensity";
        public const string Version = "1.0.1";
        public static Mod Instance { get; set; }
        private readonly static ILog _log = LogManager.GetLogger("RealisticDensity").SetShowsErrorsInUI(false);
        private World _world;

        public void OnLoad(UpdateSystem updateSystem)
        {
            _log.Info(Environment.NewLine + @":::::::::  ::::::::::     :::     :::        ::::::::::: :::::::: ::::::::::: ::::::::  
:+:    :+: :+:          :+: :+:   :+:            :+:    :+:    :+:    :+:    :+:    :+: 
+:+    +:+ +:+         +:+   +:+  +:+            +:+    +:+           +:+    +:+        
+#++:++#:  +#++:++#   +#++:++#++: +#+            +#+    +#++:++#++    +#+    +#+        
+#+    +#+ +#+        +#+     +#+ +#+            +#+           +#+    +#+    +#+        
#+#    #+# #+#        #+#     #+# #+#            #+#    #+#    #+#    #+#    #+#    #+# 
###    ### ########## ###     ### ########## ########### ########     ###     ########  
:::::::::  :::::::::: ::::    :::  :::::::: ::::::::::: ::::::::::: :::   :::           
:+:    :+: :+:        :+:+:   :+: :+:    :+:    :+:         :+:     :+:   :+:           
+:+    +:+ +:+        :+:+:+  +:+ +:+           +:+         +:+      +:+ +:+            
+#+    +:+ +#++:++#   +#+ +:+ +#+ +#++:++#++    +#+         +#+       +#++:             
+#+    +#+ +#+        +#+  +#+#+#        +#+    +#+         +#+        +#+              
#+#    #+# #+#        #+#   #+#+# #+#    #+#    #+#         #+#        #+#              
#########  ########## ###    ####  ######## ###########     ###        ###              ");
            updateSystem.UpdateAt<RealisticDensitySystem>(SystemUpdatePhase.ModificationEnd);
            _world = updateSystem.World;
        }

        private void SafelyRemove<T>()
            where T : GameSystemBase
        {
            var system = _world.GetExistingSystemManaged<T>();

            if (system != null)
                _world?.DestroySystemManaged(system);
        }

        public void OnDispose()
        {
            SafelyRemove<RealisticDensitySystem>();
        }

        public static void DebugLog(string message)
        {
            _log.Info(message);
        }
    }
}
