void run(autd3::Controller& autd) {
  const auto firm_version = autd.firmware_version();
  std::copy(firm_version.begin(), firm_version.end(),
            std::ostream_iterator<FirmwareVersion>(std::cout, "\n"));

  autd.send(Silencer{});

  Focus g(autd.center() + Vector3(0, 0, 150), FocusOption{});
  Sine m(150 * Hz, SineOption{});

  autd.send((m, g));

  std::cout << "press enter to finish..." << std::endl;
  std::cin.ignore();

  autd.close();
}