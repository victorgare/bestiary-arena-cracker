// Tick Tracker Mod for Bestiary Arena
console.log("Room cracker initializing...");

// Configuration with defaults
const defaultConfig = {
  enabled: false,
  unsubscribe: () => console.log("nothing to unsub"),
  baseUrl: "https://localhost:7284",
  speedupFactor: 10000,
  turboActive: false,
  turboSubscription: null,
};

// CONSTS
const MOD_ID = "room-cracker";
const ROOM_CRACKER_ID = `${MOD_ID}-toggle`;
const STATES = {
  initial: "initial",
  victory: "Victory",
};
const DEFAULT_TICK_INTERVAL_MS = 62.5;

function updateButtonState() {
  api.ui.updateButton(ROOM_CRACKER_ID, {
    primary: defaultConfig.enabled,
    modId: MOD_ID,
  });
}

async function getComposition() {
  const getCompositionUrl = `${defaultConfig.baseUrl}/composition`;
  const response = await fetch(getCompositionUrl);
  if (!response.ok) {
    throw new Error(`Response status: ${response.status}`);
  }

  if (response.status == 202) {
    throw new Error("There is nothing to crack :)");
  }
  return await response.json();
}

function setSandboxMode() {
  globalThis.state.board.send({ type: "setPlayMode", mode: "sandbox" });
}

// false means hide
// true means show
function setGameBoardDisplay(display = false) {
  // game board selector
  const gameFrame = document.querySelector("main .frame-4");

  gameFrame.style.display = display ? "block" : "none";
}

// Enable turbo mode to speed up the game using the more efficient approach
function enableTurbo(speedupFactor = defaultConfig.speedupFactor) {
  if (defaultConfig.turboActive) return;

  // Clean up any existing subscription
  if (defaultConfig.turboSubscription) {
    defaultConfig.turboSubscription();
    defaultConfig.turboSubscription = null;
  }

  // Calculate the new interval
  const interval = DEFAULT_TICK_INTERVAL_MS / speedupFactor;

  // Set up the subscription
  defaultConfig.turboSubscription = globalThis.state.board.on(
    "newGame",
    (event) => {
      try {
        if (event.world && event.world.tickEngine) {
          event.world.tickEngine.setTickInterval(interval);
        }
      } catch (e) {
        console.warn("Could not set tick interval in newGame event:", e);
      }
    }
  );

  // If there's an active game, try to adjust its speed but handle potential errors
  try {
    const boardContext = globalThis.state.board.getSnapshot().context;
    if (boardContext && boardContext.world && boardContext.world.tickEngine) {
      console.log(
        `Setting tick interval for existing game to ${interval}ms (${speedupFactor}x speed)`
      );
      boardContext.world.tickEngine.setTickInterval(interval);
    } else {
      console.log(
        "No active game with tickEngine found, will apply speed on next game start"
      );
    }
  } catch (e) {
    console.warn("Could not access current game tickEngine:", e);
  }

  defaultConfig.turboActive = true;
  console.log(`Turbo mode enabled (${speedupFactor}x)`);
}

// Sleep function for async operations
function sleep(ms) {
  return new Promise((resolve) => setTimeout(resolve, ms));
}

// Function to get the final result of a game run
const getLastTick = () => {
  return new Promise((resolve) => {
    let timerSubscription;
    let hasResolved = false;

    // Subscribe to timer to track ticks and game state
    timerSubscription = globalThis.state.gameTimer.subscribe((data) => {
      const { currentTick, state, readableGrade, rankPoints } = data.context;

      // Check for stop conditions
      if (state !== "initial") {
        // Game completed naturally through state change
        if (!hasResolved) {
          hasResolved = true;
          timerSubscription.unsubscribe();
          resolve({
            ticks: currentTick,
            grade: readableGrade,
            points: rankPoints,
            Victory: state === STATES.victory,
          });
        }
      }
    });
  });
};

async function sendResults(compositionId, results) {
  const response = await fetch(
    `${defaultConfig.baseUrl}/composition/${compositionId}`,
    {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(results),
    }
  );

  if (!response.ok) {
    throw new Error(`Response status: ${response.status}`);
  }
}
async function startCracking() {
  setSandboxMode();
  setGameBoardDisplay(false);
  // ensure UI is updated
  await sleep(100);
  enableTurbo();

  do {
    try {
      const { compositionId, remainingRuns, composition } =
        await getComposition();
      const results = [];

      $configureBoard(composition);

      // ensure composition is placed
      await sleep(100);

      for (let index = 1; index <= remainingRuns; index++) {
        // force stop
        if (!defaultConfig.enabled) {
          break;
        }

        // Generate a new unique seed for this run
        const runSeed = Math.floor((Date.now() * Math.random()) % 2147483647);
        console.log(`Run ${index} using generated seed: ${runSeed}`);

        // Start the game using direct state manipulation with embedded seed
        globalThis.state.board.send({
          type: "setState",
          fn: (prevState) => ({
            ...prevState,
            sandboxSeed: runSeed,
            gameStarted: true,
          }),
        });

        // Wait for game to complete
        const result = await getLastTick();

        // Add seed to result
        result.seed = runSeed;
        results.push(result);

        // Stop the game using direct state manipulation
        globalThis.state.board.send({
          type: "setState",
          fn: (prevState) => ({
            ...prevState,
            gameStarted: false,
          }),
        });

        // Brief pause between runs to ensure clean state
        await sleep(1);
      }

      console.log(`sending ${results.length} to server`);
      await sendResults(compositionId, results);
    } catch (error) {
      console.error(error);
      defaultConfig.enabled = false;
      updateButtonState();
    }
  } while (defaultConfig.enabled);
}

// Add the performance mode toggle button
api.ui.addButton({
  id: ROOM_CRACKER_ID,
  text: "Room cracker",
  modId: MOD_ID,
  primary: defaultConfig.enabled,
  onClick: () => {
    defaultConfig.enabled = !defaultConfig.enabled;
    updateButtonState();

    // only do something if enabled
    if (defaultConfig.enabled) {
      startCracking();
    }
  },
});
