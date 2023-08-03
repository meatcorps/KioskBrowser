const style = document.createElement('style');
style.innerHTML = `
   
ytmusic-app {
    width: 50vw;
}

 ytmusic-app > .ytmusic-app > * {transition: none !important;}

ytmusic-browse-response .background-gradient {
    height: calc(100vh - 173px);
    overflow: auto;
}

ytmusic-player-bar {
    width: 50vw !important;
}

div#player-bar-background {
    width: 50vw;
}

div#right-content {
    right: 50vw;
}

.center-content.style-scope.ytmusic-nav-bar {
    align-items: self-start !important;
    position: absolute !important;
    left: 250px !important;
    margin-left: 0 !important;
    transform: none !important;
}

ytmusic-player-page#player-page {
    width: calc(50vw - 240px);
}

ytmusic-player[player-ui-state="PLAYER_PAGE_OPEN"] {
    position: absolute;
    left: calc(50vw - 238px);
    top: -64px;
    height: 100vh;
    width: 50vw;
    animation: grow-into-place 0ms cubic-bezier(0.2,0,0.6,1) !important;
    max-width:  none !important;
}

ytmusic-player#player[player-ui-state="MINIPLAYER"] {
    position: fixed;
    top: calc(-100vh - 64px);
    width: 50vw;
    height: 100vh;
    left: calc(50vw - 238px);
    animation: none !important;
    z-index: 4000;
    max-width:  none !important;
}

ytmusic-player[player-ui-state="PLAYER_BAR_ONLY"] {
    position: absolute;
    top: calc(-100vh - 64px);
    height: 100vh;width: 50vw !important;
    left: calc(50vw - 238px);
    max-height: none !important;
    max-width:  none !important;
}


div#nav-bar-background {
    background: none !important;
    border: none !important;
}

ytmusic-tabbed-search-results-renderer {
    height: calc(100vh - 136px);
    overflow: auto;
}

body {
    overflow: hidden !important;
}

div#main-panel {
    width: 0px;
}

.side-panel.modular.ytmusic-player-page {
    position: absolute;
    margin-left: 0;
    width: 49vw;
    height: calc(100vh - 200px);
}

ytmusic-av-toggle {
    margin-top: -69px;
}

ytmusic-carousel#ytmusic-carousel {
    overflow-x: auto;
}
    
    img.garagelogo {position: fixed;z-index: 2000;left: calc(50vw + 30px);top: 30px;width: 300px;}

    .garageinformation {position: fixed;width: 50vw;left: 50vw;top: 80px;z-index: 1900;color: white;font-size: 2.5em;padding: 30px 30px 30px calc(330px);text-shadow: 1px 1px 2px black;background: rgb(255,0,200);
    background: linear-gradient(90deg, rgba(255,0,200,0.3) 0%, rgba(255,212,200,0) 50%);}
    
    .garageinformation span.now, .garageinformation span.next  {display: block;}
    
    .garageinformation span.next {color: #e400ff;}
    
    .garageinformation span > .info {color: rgba(255,255,255,0.5); font-size: 0.8em;}
    `;
document.head.appendChild(style);


