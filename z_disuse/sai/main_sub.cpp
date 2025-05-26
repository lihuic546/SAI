#include <iostream>

#include "autd3.hpp"
#include "autd3_link_soem.hpp"
#include "runner_0508.hpp"

using namespace autd3;

int main() try {
  auto autd = Controller::open(
      {
      //1
      AUTD3{
          .pos = Point3(AUTD3::DEVICE_WIDTH, 0, AUTD3::DEVICE_WIDTH),
          .rot = EulerAngles::ZYZ(0. * rad, - pi / 4.0 * rad, 0. * rad),
      },
      AUTD3{
          .pos = Point3::origin(),
          .rot = Quaternion::Identity(),
      },
      AUTD3{
          .pos = Point3(0, 0, AUTD3::DEVICE_WIDTH),
          .rot = Quaternion::Identity(),
      },
      AUTD3{
          .pos = Point3(0, 0, AUTD3::DEVICE_WIDTH),
          .rot = EulerAngles::ZYZ(0. * rad, pi / 4.0 * rad, 0. * rad),
      }, 
      // 2
      AUTD3{
          .pos = Point3(0, 0, AUTD3::DEVICE_WIDTH),
          .rot = EulerAngles::ZYZ(0. * rad, pi / 4.0 * rad, 0. * rad),
      }
      AUTD3{
          .pos = Point3(0, 0, AUTD3::DEVICE_WIDTH),
          .rot = Quaternion::Identity(),
      },
      AUTD3{
          .pos = Point3(0, 0, AUTD3::DEVICE_WIDTH),
          .rot = Quaternion::Identity(),
      },
      AUTD3{
          .pos = Point3(AUTD3::DEVICE_WIDTH, 0, AUTD3::DEVICE_WIDTH),
          .rot = EulerAngles::ZYZ(0. * rad, - pi / 4.0 * rad, 0. * rad),
      },
      // 3
      AUTD3{
          .pos = Point3(0, 0, AUTD3::DEVICE_WIDTH),
          .rot = EulerAngles::ZYZ(0. * rad, - pi / 4.0 * rad, 0. * rad),
      },
      AUTD3{
          .pos = Point3(0, 0, AUTD3::DEVICE_WIDTH),
          .rot = Quaternion::Identity(),
      },
      AUTD3{
          .pos = Point3(0, 0, AUTD3::DEVICE_WIDTH),
          .rot = Quaternion::Identity(),
      },
      AUTD3{
          .pos = Point3(0, 0, AUTD3::DEVICE_WIDTH),
          .rot = EulerAngles::ZYZ(0. * rad, pi / 4.0 * rad, 0. * rad),
      },},
      link::SOEM(
          [](const uint16_t slave, const link::Status status) {
            std::cout << "slave [" << slave << "]: " << status << std::endl;
            if (status == link::Status::Lost()) exit(-1);
          },
          link::SOEMOption{}));

  run(autd);
  return 0;
} catch (std::exception& ex) {
  std::cerr << ex.what() << std::endl;
}
