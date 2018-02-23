# Why not compile code as a whole for Android and so on

Consumer might be or are target platform agnostic so could be PCLs or NetStandard Libaries themselves. They would not be able to consume a target platform specific assembly. In the worst case this would lead to the necessity to provide all public APIs as stubs.

# Why not have all code in a .net standard assembly

There are certain important APIs which are not even supported in .net standard 2.0. For example NetworkInformation to enumerate interfaces.