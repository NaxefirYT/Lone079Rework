# Lone079Rework

A plugin for **SCP: Secret Laboratory** that transforms SCP-079 into a random SCP if it becomes the last SCP alive.

---

## 📖 Description

This plugin adds functionality to SCP:SL, allowing SCP-079 to transform into a random SCP (SCP-049, SCP-096, SCP-106, or SCP-939) if it remains the last SCP in the round. After transformation, SCP-079 spawns in the containment chamber of the selected SCP.

---

## 🚀 Features

- **Automatic Transformation**: SCP-079 transforms into a random SCP when it’s the last SCP alive.
- **Containment Chamber Spawn**: The transformed SCP spawns in its original containment chamber.
- **Customizable Health**: Health can scale with SCP-079’s level or remain fixed.
- **Broadcast Messages**: Players receive a broadcast message after transformation.
- **Configurable**: Fully customizable via the `config.yml` file.

---

## 🛠️ Installation

1. Download the latest release from the [Releases](https://github.com/your-username/Lone079Rework/releases) page.
2. Place the `Lone079Rework.dll` file in the `Plugins` folder of your SCP:SL server.
3. Restart the server.

---

## ⚙️ Configuration

The plugin can be configured via the `config.yml` file located in the `Configs` folder.

Example configuration:
```yaml
lone079_rework:
  is_enabled: true
  debug: false
  count_zombies: false
  scale_with_level: false
  health_percent: 50
  broadcast_message: "<i>You have been respawned as a random SCP with half health because all other SCPs have died.</i>"
  broadcast_duration: 10
  respawn_delay: 1.0
  transform_on_recontain: true
