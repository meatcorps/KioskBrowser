const style = document.createElement('style');
style.innerHTML = `
    ytmusic-app {width: 50vw !important;}
    ytmusic-app > * {width: 50vw !important;}   
    ytmusic-app > .ytmusic-app > * {width: 50vw;transition: none !important;}
    ytmusic-player[player-ui-state_="PLAYER_PAGE_OPEN"] {position: absolute;left: 50vw;top: -64px;height: 100vh;width: 50vw;animation: grow-into-place 0ms cubic-bezier(0.2,0,0.6,1) !important; max-width:  none !important;}
    ytmusic-player#player[player-ui-state_="MINIPLAYER"] {position: fixed;top: calc(-100vh - 64px);width: 50vw;height: 100vh;left: 50vw;animation: none !important;z-index: 4000; max-width:  none !important;}
    ytmusic-player[player-ui-state_="PLAYER_BAR_ONLY"] {position: absolute;top: calc(-100vh - 64px);height: 100vh;width: 50vw !important;left: 50vw;max-height: none !important;max-width:  none !important;}
    #content[role="main"] {}
    ytmusic-browse-response#browse-page {overflow: auto;height: calc(100vh - 202px);margin-top: 64px;}
    body {overflow: hidden !important;}
    ytmusic-search-page#search-page {overflow: auto;height: calc(100vh - 200px);margin-top: 64px;}
    
    img.garagelogo {position: fixed;z-index: 2000;left: calc(50vw + 30px);top: 30px;width: 300px;}

    .garageinformation {position: fixed;width: 50vw;left: 50vw;top: 80px;z-index: 1900;color: white;font-size: 2.5em;padding: 30px 30px 30px calc(330px);text-shadow: 1px 1px 2px black;background: rgb(255,0,200);
    background: linear-gradient(90deg, rgba(255,0,200,0.3) 0%, rgba(255,212,200,0) 50%);}
    
    .garageinformation span.now, .garageinformation span.next  {display: block;}
    
    .garageinformation span.next {color: #e400ff;}
    
    .garageinformation span > .info {color: rgba(255,255,255,0.5); font-size: 0.8em;}
    `;
document.head.appendChild(style);


