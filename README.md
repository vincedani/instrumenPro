# Instrumentation processing with .NET Core

### Instrumentation (C++)
```
extern "C" {
  void __cyg_profile_func_enter (void *, void *) __attribute__((no_instrument_function));
  void __cyg_profile_func_exit (void *, void *) __attribute__((no_instrument_function));

  static struct timespec time1, time2;

  void __attribute__ ((constructor)) trace_begin (void) { }

  void __attribute__ ((destructor)) trace_end (void) { }

  void __cyg_profile_func_enter (void *func,  void *caller) {
      clock_gettime(CLOCK_REALTIME, &time1);
      fprintf(stderr, "e %p %p %lu %lu\n", func, caller, time1.tv_sec, time1.tv_nsec);
  }

  void __cyg_profile_func_exit (void *func, void *caller) {
    clock_gettime(CLOCK_REALTIME, &time2);
    fprintf(stderr, "e %p %p %lu %lu\n", func, caller, time2.tv_sec, time2.tv_nsec);

  }
} // extern "C"
```
If you put this code into C++ project, it will print results to console, like this:
```
  function; caller;   realtime s; realtime ns
e 0x2518de0 0x306e4b6 1494409547  824698315
x 0x2518de0 0x306e4b6 1494409547  824777303
e 0x2851b80 0x306e4bb 1494409547  824787612
x 0x2851b80 0x306e4bb 1494409547  824790668
```
  e / x : enter / exit function
#### Reference: [Trace and profile function calls with GCC](https://balau82.wordpress.com/2010/10/06/trace-and-profile-function-calls-with-gcc/)

### Set up development enviroment:
  [Getting started with .NET Core on Windows/Linux/macOS using the command line](https://docs.microsoft.com/hu-hu/dotnet/articles/core/tutorials/using-with-xplat-cli)
  * Follow the instructions

### Usuage
  * Terminal: dotnet run `path`
  * `path` : for the instrumentation's log file
    
  
