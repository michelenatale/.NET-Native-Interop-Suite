
#include "pch.h"
#include <stdio.h>
#include <string.h>
#include <iostream>
#include "shared_ringbuffer.h"

#pragma comment(lib, "SidecarModelLib.lib")

enum command_type : uint8_t
{
  cmd_process_data = 1,
  cmd_ping = 2,
  cmd_shutdown = 3
};


int main(int argc, char** argv)
{
  if (argc < 3) return 1;

  const char* name = argv[1];
  uint32_t capacity = (uint32_t)atoi(argv[2]);

  shared_rb_t* rb = shared_rb_open(name);
  if (!rb) return 2;

  uint8_t buffer[4096];

  bool running = true;
  while (running)
  {
    uint32_t read = shared_rb_read(rb, buffer, sizeof(buffer));
    if (read == 0)
    {
      // Sleep / yield
      continue;
    }

    uint8_t cmd = buffer[0];
    uint32_t len;
    memcpy(&len, buffer + 1, 4);

    uint8_t* payload = buffer + 5;

    switch (cmd)
    {
    case cmd_process_data:
      // hier echte Arbeit
      printf("[Sidecar] ProcessData: %u bytes\n", len);
      break;
    case cmd_ping:
      printf("[Sidecar] Ping\n");
      break;
    case cmd_shutdown:
      running = false;
      break;
    }
  }

  shared_rb_close(rb);
  return 0;

}
 